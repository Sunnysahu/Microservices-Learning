using System.Net;

namespace OrderService.Services
{
    public class InventoryClient : IInventoryClient
    {
        private readonly HttpClient _httpClient;

        public InventoryClient(HttpClient httpClient) => _httpClient = httpClient;

        public async Task<bool> ReserveStockBatchAsync(List<(int ProductId, int Quantity)> items, CancellationToken cancellationToken)
        {
            var request = new
            {
                items = items.Select(item => new
                {
                    productId = item.ProductId,
                    quantity = item.Quantity
                }).ToList()
            };

            var response = await _httpClient
                .PostAsJsonAsync("api/Inventory/reserve-batch", request, cancellationToken);

            if (response.StatusCode == HttpStatusCode.BadRequest) return false;

            response.EnsureSuccessStatusCode();

            return true;
        }
    }
}
