using PaymentService.Models;

namespace PaymentService.Repositories
{
    public interface IPaymentRepository
    {
        Task<Payment?> GetByOrderIdAsync(int orderId, CancellationToken cancellationToken);

        Task<Payment?> GetByIdempotencyKeyAsync(string idempotencyKey, CancellationToken cancellationToken);

        Task<Payment> CreateAsync(Payment payment, CancellationToken cancellationToken);
    }
}
