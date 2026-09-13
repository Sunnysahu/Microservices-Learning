using InventoryService.Models;
using InventoryService.Repositories;

namespace InventoryService.Services
{
    public class InventoryManager : IInventoryService
    {
        private readonly IInventoryRepository _repository;
            
        public InventoryManager(IInventoryRepository repository) =>_repository = repository;

        public async Task<InventoryItem?> GetByProductIdAsync(int productId, CancellationToken cancellationToken)
        {
            return await _repository.GetByProductIdAsync(productId, cancellationToken);
        }

        public async Task<List<InventoryItem>> GetAllAsync(CancellationToken cancellationToken)
        {
            return await _repository.GetAllAsync(cancellationToken);
        }
    }
}
