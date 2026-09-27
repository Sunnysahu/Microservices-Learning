using OrderService.Models;

namespace OrderService.DTOs
{
    public enum CreateOrderStatus
    {
        Success,
        ProductNotFound,
        InsufficientStock
    }

    public class CreateOrderResult
    {
        public CreateOrderStatus Status { get; set; }

        public string Message { get; set; } = string.Empty;

        public Order? Order { get; set; }
    }
}
