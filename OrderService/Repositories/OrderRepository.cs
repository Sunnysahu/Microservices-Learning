using Microsoft.EntityFrameworkCore;
using OrderService.Data;
using OrderService.Models;
using System.Text.Json;

namespace OrderService.Repositories
{
    public class OrderRepository(OrderDbContext context) : IOrderRepository
    {
        private readonly OrderDbContext _context = context;

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



        public async Task<Order> CreateAsync(Order order, string paymentIdempotencyKey, decimal totalAmount, CancellationToken cancellationToken)
        {
            await using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);

            try
            {
                _context.Orders.Add(order);
                await _context.SaveChangesAsync(cancellationToken);

                var outboxMessage = new OutboxMessage
                {
                    EventType = "OrderCreated",
                    CreatedAt = DateTime.Now,
                    Payload = JsonSerializer.Serialize(new
                    {
                        OrderId = order.Id,
                        CustomerName = order.CustomerName,
                        PaymentIdempotencyKey = paymentIdempotencyKey,
                        Items = order.Items.Select(item => new
                        {
                            item.ProductId,
                            item.Quantity,
                            item.UnitPrice
                        }),
                        TotalAmount = totalAmount
                    })
                };

                _context.OutboxMessages.Add(outboxMessage);

                await _context.SaveChangesAsync(cancellationToken);

                await transaction.CommitAsync(cancellationToken);

                return order;
            }
            catch
            {
                await transaction.RollbackAsync(cancellationToken);
                throw;
            }
        }


        public async Task UpdateAsync(Order order, CancellationToken cancellationToken)
        {
            _context.Orders.Update(order);

            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
