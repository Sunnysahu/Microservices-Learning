namespace OrderService.Services
{
    public interface IInventoryClient
    {
        Task<InventoryReservationResult> ReserveStockBatchAsync(List<(int ProductId, int Quantity)> items, CancellationToken cancellationToken);
    }
}
