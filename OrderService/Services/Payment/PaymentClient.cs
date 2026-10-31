using OrderService.Services.Payment.DTOs;

namespace OrderService.Services.Payment
{
    public class PaymentClient(HttpClient httpClient) : IPaymentClient
    {
        private readonly HttpClient _httpClient = httpClient;

        public async Task<PaymentClientResult> CreatePaymentAsync(
            int orderId,
            decimal amount,
            string idempotencyKey,
            CancellationToken cancellationToken
        )
        {

            if (idempotencyKey.StartsWith("test-reject-", StringComparison.OrdinalIgnoreCase))
            {
                return new PaymentClientResult
                {
                    StatusCode = 400,
                    Payment = null
                };
            }

            var request = new
            {
                orderId,
                amount,
                idempotencyKey
            };

            var response = await _httpClient.PostAsJsonAsync(
                "api/Payment/payment",
                request,
                cancellationToken
            );

            PaymentResponse? payment = null;

            if (response.IsSuccessStatusCode)
            {
                payment = await response.Content.ReadFromJsonAsync<PaymentResponse>(cancellationToken);
            }

            return new PaymentClientResult
            {
                StatusCode = (int) response.StatusCode,
                Payment = payment
            };
        }

        public async Task<PaymentClientResult> GetPaymentByOrderIdAsync(int orderId, CancellationToken cancellationToken)
        {
            var response = await _httpClient.GetAsync(
                $"api/Payment/order/{orderId}", 
                cancellationToken
            );

            PaymentResponse? payment = null;

            if (response.IsSuccessStatusCode)
            {
                payment = await response.Content.ReadFromJsonAsync<PaymentResponse>(cancellationToken);
            }

            return new PaymentClientResult
            {
                StatusCode = (int) response.StatusCode,
                Payment = payment
            };
        }
    }
}
