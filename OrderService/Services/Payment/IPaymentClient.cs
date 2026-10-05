using OrderService.Services.Payment.DTOs;

namespace OrderService.Services.Payment
{
    public interface IPaymentClient
    {
        Task<PaymentResponse?> CreatePaymentAsync(
            int orderId,
            decimal amount,
            string idempotencyKey,
            CancellationToken cancellationToken
        );
    }
}
