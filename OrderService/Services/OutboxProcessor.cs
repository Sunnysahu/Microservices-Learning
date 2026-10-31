using Microsoft.EntityFrameworkCore;
using OrderService.Data;
using OrderService.Services.Inventory;
using OrderService.Services.Payment;
using System.Text.Json;

namespace OrderService.Services;

public class OutboxProcessor(IServiceScopeFactory scopeFactory, ILogger<OutboxProcessor> logger) : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory = scopeFactory;
    private readonly ILogger<OutboxProcessor> _logger = logger;

    private const int MaxRetryCount = 5;
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var lastRecoveryCheck = DateTime.Now;
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using var scope = _scopeFactory.CreateScope();

                var paymentClient = scope.ServiceProvider.GetRequiredService<IPaymentClient>();

                var inventoryClient = scope.ServiceProvider.GetRequiredService<IInventoryClient>();

                var context = scope.ServiceProvider.GetRequiredService<OrderDbContext>();

                var messages = await context.OutboxMessages
                    .Where(x => x.ProcessedAt == null && x.FailedAt == null && x.RetryCount < MaxRetryCount)
                    .OrderBy(x => x.Id)
                    .Take(10)
                    .ToListAsync(stoppingToken);

                if (DateTime.Now - lastRecoveryCheck >= TimeSpan.FromMinutes(1))
                {
                    var recoveryMessages = await context.OutboxMessages
                        .Where(x => x.ProcessedAt == null && x.FailedAt == null && x.RetryCount >= MaxRetryCount)
                        .OrderBy(x => x.Id)
                        .Take(10)
                        .ToListAsync(stoppingToken);

                    messages.AddRange(recoveryMessages);
                    lastRecoveryCheck = DateTime.Now;
                }

                foreach (var message in messages)
                {
                    try
                    {
                        _logger.LogInformation(
                            "Processing Outbox Message {MessageId}: {EventType}",
                            message.Id,
                            message.EventType
                        );

                        if (message.EventType != "OrderCreated")
                        {
                            _logger.LogError(
                                "Outbox message {MessageId} has unsupported event type {EventType}. Marking it as failed.",
                                message.Id,
                                message.EventType
                            );

                            message.FailedAt = DateTime.Now;

                            await context.SaveChangesAsync(stoppingToken);
                            continue;
                        }

                        var payload = JsonSerializer.Deserialize<OrderCreatedEvent>(message.Payload);

                        if (payload is null || string.IsNullOrWhiteSpace(payload.PaymentIdempotencyKey) || 
                            payload.Items is null || payload.Items.Count == 0)
                        {
                            _logger.LogError(
                                "Outbox message {MessageId} has an invalid payload. Marking it as failed.", message.Id);


                            message.FailedAt = DateTime.Now;


                            await context.SaveChangesAsync(stoppingToken);
                            continue;
                        }

                        var order = await context.Orders
                            .FirstOrDefaultAsync(x => x.Id == payload.OrderId, stoppingToken);

                        if (order is null)
                        {
                            _logger.LogError(
                                "Order {OrderId} for Outbox message {MessageId} was not found. Marking it as failed.",
                                payload.OrderId,
                                message.Id
                            );

                            message.FailedAt = DateTime.Now;

                            await context.SaveChangesAsync(stoppingToken);
                            continue;
                        }

                        var paymentLookup = await 
                            paymentClient.GetPaymentByOrderIdAsync(payload.OrderId, stoppingToken);

                        if (paymentLookup.IsSuccess && 
                            paymentLookup.Payment is not null &&
                            paymentLookup.Payment.OrderId == payload.OrderId && 
                            paymentLookup.Payment.Amount == payload.TotalAmount &&
                            string.Equals(paymentLookup.Payment.Status, "Paid", StringComparison.OrdinalIgnoreCase))
                        {
                            order.Status = "Paid";
                            message.ProcessedAt = DateTime.Now;

                            await context.SaveChangesAsync(stoppingToken);

                            _logger.LogInformation(
                                "Recovered Outbox message {MessageId}: payment already exists for Order {OrderId}.",
                                message.Id,
                                payload.OrderId
                            );

                            continue;
                        }

                        if (paymentLookup.IsSuccess && 
                            paymentLookup.Payment is not null && 
                            (paymentLookup.Payment.OrderId != payload.OrderId || 
                            paymentLookup.Payment.Amount != payload.TotalAmount)
                        )
                        { 
                            _logger.LogError(
                                "Payment mismatch for Order {OrderId}, Outbox message {MessageId}. " +
                                "Expected amount {ExpectedAmount}, received {ActualAmount}.",
                                payload.OrderId, message.Id, payload.TotalAmount, paymentLookup.Payment.Amount
                            );

                            message.FailedAt = DateTime.Now;

                            await context.SaveChangesAsync(stoppingToken);
                            continue;
                        }

                        if (paymentLookup.StatusCode != 404 && !paymentLookup.IsSuccess)
                        {
                            throw new HttpRequestException(
                                $"Payment lookup returned HTTP {paymentLookup.StatusCode} " +
                                $"for Order {payload.OrderId}."
                            );
                        }

                        if (message.RetryCount >= 5)
                        {
                            _logger.LogWarning(
                                "Outbox message {MessageId} has exhausted retries. " +
                                "Payment was not found for Order {OrderId}; attempting recovery with the same idempotency key.",
                                message.Id,
                                payload.OrderId
                            );
                        }

                        if (order.Status == "Paid" || order.Status == "PaymentFailed")
                        {
                            message.ProcessedAt = DateTime.Now;

                            await context.SaveChangesAsync(stoppingToken);

                            _logger.LogInformation("Outbox message {MessageId} was already handled.", message.Id);

                            continue;
                        }

                        var paymentResult = await 
                            paymentClient.CreatePaymentAsync(
                                payload.OrderId,
                                payload.TotalAmount,
                                payload.PaymentIdempotencyKey,
                                stoppingToken
                            );

                        if (paymentResult.IsSuccess)
                        {
                            order.Status = "Paid";
                            message.ProcessedAt = DateTime.Now;
                        }
                        else if (paymentResult.StatusCode == 400 || paymentResult.StatusCode == 422)
                        {
                            _logger.LogWarning(
                                "Payment rejected for Order {OrderId}. HTTP {StatusCode}",
                                payload.OrderId,
                                paymentResult.StatusCode
                            );

                            var items = payload.Items
                                .Select(item => (item.ProductId, item.Quantity))
                                .ToList();

                            var released = await 
                                inventoryClient.ReleaseStockBatchAsync(payload.OrderId, items, stoppingToken);

                            if (!released)
                            {
                                throw new HttpRequestException($"Failed to release inventory for Order {payload.OrderId}.");
                            }

                            // Only mark the order/message complete after
                            // inventory release succeeds.
                            order.Status = "PaymentFailed";
                            message.ProcessedAt = DateTime.Now;
                        }
                        else
                        {
                            throw new HttpRequestException($"PaymentService returned HTTP {paymentResult.StatusCode}.");
                        }

                        await context.SaveChangesAsync(stoppingToken);

                        _logger.LogInformation("Outbox message {MessageId} handled.", message.Id);
                    }
                    catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
                    {
                        throw;
                    }
                    catch (Exception ex)
                    {
                        if (message.RetryCount < MaxRetryCount)
                        {
                            message.RetryCount++;
                        }

                        _logger.LogError(
                            ex,
                            "Outbox message {MessageId} failed on attempt {RetryCount}.",
                            message.Id,
                            message.RetryCount
                        );

                        if (message.RetryCount >= MaxRetryCount)
                        {
                            _logger.LogError(
                                "Outbox message {MessageId} reached the retry limit. " 
                                + "It remains pending and requires recovery.",
                                message.Id
                            );
                        }

                        await context.SaveChangesAsync(stoppingToken);
                    }
                }
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                break;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing outbox batch.");
            }

            try
            {
                await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                break;
            }
        }
    }

    public class OrderCreatedEvent
    {
        public int OrderId { get; set; }

        public string CustomerName { get; set; } = string.Empty;

        public string PaymentIdempotencyKey { get; set; } = string.Empty;

        public decimal TotalAmount { get; set; }

        public List<OrderCreatedEventItem> Items { get; set; } = [];
    }

    public class OrderCreatedEventItem
    {
        public int ProductId { get; set; }

        public int Quantity { get; set; }

        public decimal UnitPrice { get; set; }
    }
}
