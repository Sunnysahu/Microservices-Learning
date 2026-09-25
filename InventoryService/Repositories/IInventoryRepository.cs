using InventoryService.DTOs;
using InventoryService.Models;

namespace InventoryService.Repositories
{
    public interface IInventoryRepository
    {
        Task<InventoryItem?> GetByProductIdAsync(int productId, CancellationToken cancellationToken);

        Task<List<InventoryItem>> GetAllAsync(CancellationToken cancellationToken);

        Task<ReserveStockResult> ReserveStockBatchAsync(List<(int ProductId, int Quantity)> items, 
            CancellationToken cancellationToken);
    }
}
