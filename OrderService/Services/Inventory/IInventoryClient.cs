namespace OrderService.Services.Inventory
{
    public interface IInventoryClient
    {
        Task<InventoryReservationResult> ReserveStockBatchAsync(List<(int ProductId, int Quantity)> items, CancellationToken cancellationToken);

        Task<bool> ReleaseStockBatchAsync(int orderId, List<(int ProductId, int Quantity)> items, CancellationToken cancellationToken);
    }
}
