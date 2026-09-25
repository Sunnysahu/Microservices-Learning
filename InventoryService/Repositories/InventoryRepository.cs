using InventoryService.Data;
using InventoryService.DTOs;
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
            return await _context.Inventory.AsNoTracking().ToListAsync(cancellationToken);
        }

        public async Task<ReserveStockResult> ReserveStockBatchAsync(List<(int ProductId, int Quantity)> items,
        CancellationToken cancellationToken)
        {
            await using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);

            try
            {
                var productIds = items
                    .Select(x => x.ProductId)
                    .Distinct()
                    .ToList();

                var inventoryItems =
                    await _context.Inventory
                    .Where(x => productIds.Contains(x.ProductId))
                    .ToListAsync(cancellationToken);

                foreach (var item in items)
                {
                    var inventoryItem = inventoryItems.FirstOrDefault(x => x.ProductId == item.ProductId);

                    if (inventoryItem is null)
                    {
                        await transaction.RollbackAsync(cancellationToken);

                        return new ReserveStockResult
                        {
                            Status = ReserveStockStatus.ProductNotFound,
                            Message = $"Product {item.ProductId} was not found."
                        };
                    }
                    if (inventoryItem.StockQuantity < item.Quantity)
                    {
                        await transaction.RollbackAsync(cancellationToken);

                        return new ReserveStockResult
                        {
                            Status = ReserveStockStatus.InsufficientStock,
                            Message = $"Insufficient stock for Product {item.ProductId}."
                        };
                    }
                }

                return new ReserveStockResult
                {
                    Status = ReserveStockStatus.Success,
                    Message = "Stock reserved successfully."
                };
            }
            catch
            {
                await transaction.RollbackAsync(cancellationToken);
                throw;
            }
        }
    }
}
