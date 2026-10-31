using OrderService.Services.Payment.DTOs;

namespace OrderService.Services.Payment
{
    public interface IPaymentClient
    {
        Task<PaymentClientResult> CreatePaymentAsync(
            int orderId,
            decimal amount,
            string idempotencyKey,
            CancellationToken cancellationToken
        );

        Task<PaymentClientResult> GetPaymentByOrderIdAsync(int orderId, CancellationToken cancellationToken);
    }
}
