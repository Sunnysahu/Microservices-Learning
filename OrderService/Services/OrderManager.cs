using OrderService.DTOs;
using OrderService.Models;
using OrderService.Repositories;
using OrderService.Services.Inventory;
using OrderService.Services.Payment;

namespace OrderService.Services
{
    public class OrderManager : IOrderService
    {
        private readonly IOrderRepository _repository;
        private readonly IInventoryClient _inventoryClient;
        private readonly IPaymentClient _paymentClient;

        public OrderManager(IOrderRepository repository, IInventoryClient inventoryClient, IPaymentClient paymentClient)
            => (_repository, _inventoryClient, _paymentClient) = (repository, inventoryClient, paymentClient);

        public async Task<List<Order>> GetAllAsync(CancellationToken cancellationToken)
        {
            return await _repository.GetAllAsync(cancellationToken);
        }

        public async Task<Order?> GetByIdAsync(int id, CancellationToken cancellationToken)
        {
            return await _repository.GetByIdAsync(id, cancellationToken);
        }

        public async Task<CreateOrderResult> CreateAsync(CreateOrderRequest request, CancellationToken cancellationToken)
        {
            var items = request
                .Items
                .Select(item => (item.ProductId, item.Quantity))
                .ToList();

            var totalAmount = request.Items.Sum(item => item.Quantity * item.UnitPrice);

            var reservationResult =
                await _inventoryClient.ReserveStockBatchAsync(items, cancellationToken);

            if (reservationResult.Status == InventoryReservationStatus.ProductNotFound)
            {
                return new CreateOrderResult
                {
                    Status = CreateOrderStatus.ProductNotFound,
                    Message = reservationResult.Message
                };
            }

            if (reservationResult.Status == InventoryReservationStatus.InsufficientStock)
            {
                return new CreateOrderResult
                {
                    Status = CreateOrderStatus.InsufficientStock,
                    Message = reservationResult.Message
                };
            }

            var order = new Order
            {
                CustomerName = request.CustomerName,
                Status = "Pending",
                CreatedAt = DateTime.Now,
                Items = request.Items
                    .Select(item => new OrderItem
                    {
                        ProductId = item.ProductId,
                        Quantity = item.Quantity,
                        UnitPrice = item.UnitPrice
                    }).ToList()
            };

            var outboxMessage = new OutboxMessage
            {
                EventType = "OrderCreated",
                CreatedAt = DateTime.Now
            };

            var createdOrder = await _repository.CreateAsync(order, outboxMessage, cancellationToken);

            // Payment Microservice Call
            try
            {
                var payment = await _paymentClient.CreatePaymentAsync(
                    createdOrder.Id,
                    totalAmount,
                    request.PaymentIdempotencyKey,
                    cancellationToken
                );
            }
            catch (HttpRequestException)
            {

                Console.WriteLine("PAYMENT FAILED");

                createdOrder.Status = "PaymentFailed";

                await _repository.UpdateAsync(createdOrder, cancellationToken);

                Console.WriteLine("ORDER MARKED PAYMENT FAILED");

                var released = await _inventoryClient.ReleaseStockBatchAsync(createdOrder.Id, items, cancellationToken);

                Console.WriteLine($"STOCK RELEASE RESULT: {released}");

                throw;
            }

            createdOrder.Status = "Paid";

            await _repository.UpdateAsync(createdOrder, cancellationToken);

            return new CreateOrderResult
            {
                Status = CreateOrderStatus.Success,
                Message = "Order created and payment completed successfully.",
                Order = createdOrder
            };
        }
    }
}