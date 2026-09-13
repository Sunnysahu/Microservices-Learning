using InventoryService.Data;
using InventoryService.Models;
using Microsoft.EntityFrameworkCore;

namespace InventoryService.Repositories
{
    public class InventoryRepository : IInventoryRepository
    {
        private readonly InventoryDbContext _context;

        public InventoryRepository(InventoryDbContext context)
        {
            _context = context;
        }

        public async Task<InventoryItem?> GetByProductIdAsync(int productId, CancellationToken cancellationToken)
        {
            return await _context.Inventory
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.ProductId == productId, cancellationToken);
        }

        public async Task<List<InventoryItem>> GetAllAsync(CancellationToken cancellationToken)
        {
            return await _context.Inventory
                .AsNoTracking()
                .ToListAsync(cancellationToken);
        }
    }
}
