using OrderService.DTOs;
using OrderService.Models;
using OrderService.Repositories;

namespace OrderService.Services
{
    public class OrderManager : IOrderService
    {
        private readonly IOrderRepository _repository;

        public OrderManager(IOrderRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<Order>> GetAllAsync(CancellationToken cancellationToken)
        {
            return await _repository.GetAllAsync(cancellationToken);
        }

        public async Task<Order?> GetByIdAsync(int id, CancellationToken cancellationToken)
        {
            return await _repository.GetByIdAsync(id, cancellationToken);
        }

        public async Task<Order> CreateAsync(CreateOrderRequest request, CancellationToken cancellationToken)
        {
            var order = new Order
            {
                CustomerName = request.CustomerName,
                Status = "Pending",
                CreatedAt = DateTime.Now,

                Items = request.Items.Select(item => new OrderItem
                {
                    ProductId = item.ProductId,
                    Quantity = item.Quantity
                }).ToList()
            };

            return await _repository.CreateAsync(order, cancellationToken);
        }
    }
}
