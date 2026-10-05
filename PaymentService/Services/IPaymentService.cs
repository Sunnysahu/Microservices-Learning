using PaymentService.DTOs;

namespace PaymentService.Services
{
    public interface IPaymentService
    {
        Task<CreatePaymentResult> CreateAsync(CreatePaymentRequest request, CancellationToken cancellationToken);

        Task<PaymentResponse?> GetByOrderIdAsync(int orderId, CancellationToken cancellationToken);
    }
}
