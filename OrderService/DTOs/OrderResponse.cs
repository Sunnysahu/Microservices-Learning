namespace OrderService.DTOs
{
    public class OrderResponse
    {
        public int Id { get; set; }

        public string CustomerName { get; set; } = string.Empty;

        public string Status { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; }

        public List<OrderItemResponse> Items { get; set; } = new();
    }
}
