using InventoryService.Models;

namespace InventoryService.Repositories
{
    public interface IInventoryRepository
    {
        Task<InventoryItem?> GetByProductIdAsync(int productId, CancellationToken cancellationToken);

        Task<List<InventoryItem>> GetAllAsync(CancellationToken cancellationToken);
    }
}
