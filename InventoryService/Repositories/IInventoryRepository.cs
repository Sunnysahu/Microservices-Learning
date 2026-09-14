using InventoryService.Models;

namespace InventoryService.Repositories
{
    public interface IInventoryRepository
    {
        Task<InventoryItem?> GetByProductIdAsync(int productId, CancellationToken cancellationToken);

        Task<List<InventoryItem>> GetAllAsync(CancellationToken cancellationToken);

        Task<bool> ReserveStockAsync(int productId, int quantity, CancellationToken cancellationToken);
    }
}
