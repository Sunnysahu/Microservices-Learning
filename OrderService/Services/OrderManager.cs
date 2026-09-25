using OrderService.DTOs;
using OrderService.Models;
using OrderService.Repositories;

namespace OrderService.Services
{
    public class OrderManager : IOrderService
    {
        private readonly IOrderRepository _repository;
        private readonly IInventoryClient _inventoryClient;

        public OrderManager(IOrderRepository repository, IInventoryClient inventoryClient) => 
            (_repository, _inventoryClient) = (repository, inventoryClient);

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

            var items = request.Items
                .Select(item => (item.ProductId, item.Quantity))
                .ToList();

            var reserved = await _inventoryClient.ReserveStockBatchAsync(items, cancellationToken);

            if (!reserved)
            {
                throw new InvalidOperationException("Could not reserve stock for the order.");
            }

            var order = new Order
            {
                CustomerName = request.CustomerName,
                Status = "Pending",
                CreatedAt = DateTime.Now,

                Items = [.. request.Items.Select(item => new OrderItem
                {
                    ProductId = item.ProductId,
                    Quantity = item.Quantity
                })]
            };

            return await _repository.CreateAsync(order, cancellationToken);
        }
    }
}
