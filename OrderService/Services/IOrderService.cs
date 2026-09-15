using OrderService.DTOs;
using OrderService.Models;

namespace OrderService.Services
{
    public interface IOrderService
    {
        Task<List<Order>> GetAllAsync(CancellationToken cancellationToken);

        Task<Order?> GetByIdAsync(int id, CancellationToken cancellationToken);

        Task<Order> CreateAsync(CreateOrderRequest request, CancellationToken cancellationToken);
    }
}
