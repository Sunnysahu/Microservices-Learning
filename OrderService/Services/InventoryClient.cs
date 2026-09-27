using System.Net;

namespace OrderService.Services
{
    public class InventoryClient : IInventoryClient
    {
        private readonly HttpClient _httpClient;

        public InventoryClient(HttpClient httpClient) => _httpClient = httpClient;

        public async Task<InventoryReservationResult> ReserveStockBatchAsync(List<(int ProductId, int Quantity)> items, CancellationToken cancellationToken)
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

            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                return new InventoryReservationResult
                {
                    Status = InventoryReservationStatus.ProductNotFound,
                    Message = "One or more products were not found."
                };
            }

            if (response.StatusCode == HttpStatusCode.Conflict)
            {
                return new InventoryReservationResult
                {
                    Status = InventoryReservationStatus.InsufficientStock,
                    Message = "Insufficient stock."
                };
            }

            response.EnsureSuccessStatusCode();

            return new InventoryReservationResult
            {
                Status = InventoryReservationStatus.Success,
                Message = "Stock reserved successfully."
            };
        }
    }
}
