using Microsoft.EntityFrameworkCore;
using OrderService.Data;
using OrderService.Models;

namespace OrderService.Repositories
{
    public class OrderRepository : IOrderRepository
    {
        private readonly OrderDbContext _context;

        public OrderRepository(OrderDbContext context)
        {
            _context = context;
        }

        public async Task<List<Order>> GetAllAsync(CancellationToken cancellationToken)
        {
            return await _context.Orders
                .AsNoTracking()
                .Include(x => x.Items)
                .ToListAsync(cancellationToken);
        }

        public async Task<Order?> GetByIdAsync(int id, CancellationToken cancellationToken)
        {
            return await _context.Orders
                .AsNoTracking()
                .Include(x => x.Items)
                .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        }

        public async Task<Order> CreateAsync(Order order, CancellationToken cancellationToken)
        {
            _context.Orders.Add(order);

            await _context.SaveChangesAsync(cancellationToken);

            return order;
        }
    }
}
