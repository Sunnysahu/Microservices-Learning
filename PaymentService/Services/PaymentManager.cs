using PaymentService.DTOs;
using PaymentService.Models;
using PaymentService.Repositories;

namespace PaymentService.Services
{
    public class PaymentManager : IPaymentService
    {
        private readonly IPaymentRepository _repository;

        public PaymentManager(IPaymentRepository repository) => _repository = repository;

        public async Task<PaymentResponse> CreateAsync(CreatePaymentRequest request, CancellationToken cancellationToken)
        {
            var existingPayment =
                await _repository.GetByIdempotencyKeyAsync(request.IdempotencyKey, cancellationToken);

            if (existingPayment is not null)
            {
                return new PaymentResponse
                {
                    Id = existingPayment.Id,
                    OrderId = existingPayment.OrderId,
                    Amount = existingPayment.Amount,
                    Status = existingPayment.Status,
                    IdempotencyKey = existingPayment.IdempotencyKey,
                    CreatedAt = existingPayment.CreatedAt
                };
            }

            var payment = new Payment
            {
                OrderId = request.OrderId,
                Amount = request.Amount,
                Status = "Paid",
                IdempotencyKey = request.IdempotencyKey,
                CreatedAt = DateTime.UtcNow
            };

            var createdPayment = await _repository.CreateAsync(payment, cancellationToken);

            return new PaymentResponse
            {
                Id = createdPayment.Id,
                OrderId = createdPayment.OrderId,
                Amount = createdPayment.Amount,
                Status = createdPayment.Status,
                IdempotencyKey = createdPayment.IdempotencyKey,
                CreatedAt = createdPayment.CreatedAt
            };
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
