using OrderService.Models;

namespace OrderService.Repositories
{
    public interface IOrderRepository
    {
        Task<List<Order>> GetAllAsync(CancellationToken cancellationToken);

        Task<Order?> GetByIdAsync(int id, CancellationToken cancellationToken);

        Task<Order> CreateAsync(Order order, CancellationToken cancellationToken);
    }
}
