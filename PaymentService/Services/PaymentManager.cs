using Microsoft.EntityFrameworkCore;
using PaymentService.DTOs;
using PaymentService.Models;
using PaymentService.Repositories;

namespace PaymentService.Services
{
    public class PaymentManager : IPaymentService
    {
        private readonly IPaymentRepository _repository;

        public PaymentManager(IPaymentRepository repository) => _repository = repository;

        public async Task<CreatePaymentResult> CreateAsync(CreatePaymentRequest request, CancellationToken cancellationToken)
        {
            var existingPayment =
                await _repository.GetByIdempotencyKeyAsync(request.IdempotencyKey, cancellationToken);

            if (existingPayment is not null)
            {
                if (existingPayment.OrderId != request.OrderId ||
                    existingPayment.Amount != request.Amount)
                {
                    return new CreatePaymentResult
                    {
                        Status = CreatePaymentStatus.IdempotencyConflict,
                        Message = "The idempotency key was already used for a different payment."
                    };
                }

                return new CreatePaymentResult
                {
                    Status = CreatePaymentStatus.Success,
                    Message = "Existing payment returned.",
                    Payment = new PaymentResponse
                    {
                        Id = existingPayment.Id,
                        OrderId = existingPayment.OrderId,
                        Amount = existingPayment.Amount,
                        Status = existingPayment.Status,
                        IdempotencyKey = existingPayment.IdempotencyKey,
                        CreatedAt = existingPayment.CreatedAt
                    }
                };
            }

            var payment = new Payment
            {
                OrderId = request.OrderId,
                Amount = request.Amount,
                Status = "Paid",
                IdempotencyKey = request.IdempotencyKey,
                CreatedAt = DateTime.Now
            };

            try
            {
                var createdPayment = await _repository.CreateAsync(payment, cancellationToken);

                return new CreatePaymentResult
                {
                    Status = CreatePaymentStatus.Success,
                    Message = "Payment created successfully.",
                    Payment = new PaymentResponse
                    {
                        Id = createdPayment.Id,
                        OrderId = createdPayment.OrderId,
                        Amount = createdPayment.Amount,
                        Status = createdPayment.Status,
                        IdempotencyKey = createdPayment.IdempotencyKey,
                        CreatedAt = createdPayment.CreatedAt
                    }
                };
            }
            catch (DbUpdateException ex)
                when (ex.InnerException?.Message.Contains(
                    "UX_Payments_IdempotencyKey", 
                    StringComparison.OrdinalIgnoreCase) == true
                )
            {
                var existingPaymentAfterConflict =
                    await _repository.GetByIdempotencyKeyAsync(request.IdempotencyKey, cancellationToken);

                if (existingPaymentAfterConflict is not null)
                {
                    return new CreatePaymentResult
                    {
                        Status = CreatePaymentStatus.Success,
                        Message = "Existing payment returned.",
                        Payment = new PaymentResponse
                        {
                            Id = existingPaymentAfterConflict.Id,
                            OrderId = existingPaymentAfterConflict.OrderId,
                            Amount = existingPaymentAfterConflict.Amount,
                            Status = existingPaymentAfterConflict.Status,
                            IdempotencyKey = existingPaymentAfterConflict.IdempotencyKey,
                            CreatedAt = existingPaymentAfterConflict.CreatedAt
                        }
                    };
                }

                throw;
            }
        }

        public async Task<PaymentResponse?> GetByOrderIdAsync(int orderId, CancellationToken cancellationToken)
        {
            var payment = await _repository.GetByOrderIdAsync(orderId, cancellationToken);

            if (payment is null)
            {
                return null;
            }

            return new PaymentResponse
            {
                Id = payment.Id,
                OrderId = payment.OrderId,
                Amount = payment.Amount,
                Status = payment.Status,
                IdempotencyKey = payment.IdempotencyKey,
                CreatedAt = payment.CreatedAt
            };
        }
    }
}
