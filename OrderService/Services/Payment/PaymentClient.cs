using OrderService.Services.Payment.DTOs;

namespace OrderService.Services.Payment
{
    public class PaymentClient : IPaymentClient
    {
        private readonly HttpClient _httpClient;

        public PaymentClient(HttpClient httpClient) => _httpClient = httpClient;

        public async Task<PaymentResponse?> CreatePaymentAsync(
            int orderId,
            decimal amount,
            string idempotencyKey,
            CancellationToken cancellationToken
        )
        {
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

            response.EnsureSuccessStatusCode();

            return await response.Content.ReadFromJsonAsync<PaymentResponse>(cancellationToken);
        }
    }
}
