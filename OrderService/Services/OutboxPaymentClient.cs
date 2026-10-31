namespace OrderService.Services;

public class OutboxPaymentClient(HttpClient httpClient)
{
    private readonly HttpClient _httpClient = httpClient;

    public async Task ProcessPaymentAsync(int orderId, decimal amount, string idempotencyKey, CancellationToken cancellationToken)
    {
        var request = new
        {
            orderId,
            amount,
            idempotencyKey
        };

        var response = await _httpClient.PostAsJsonAsync("api/Payment", request, cancellationToken);

        response.EnsureSuccessStatusCode();
    }
}