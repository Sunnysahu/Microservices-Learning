namespace OrderService.Services
{
    public interface IInventoryClient
    {
        Task<bool> ReserveStockBatchAsync(List<(int ProductId, int Quantity)> items, CancellationToken cancellationToken);
    }
}
