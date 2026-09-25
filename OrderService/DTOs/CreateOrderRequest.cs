using System.ComponentModel.DataAnnotations;

namespace OrderService.DTOs
{
    public class CreateOrderRequest
    {
        [Required(AllowEmptyStrings = false, ErrorMessage = "User Must Enter the Name")]
        public string CustomerName { get; set; } = string.Empty;

        public List<CreateOrderItemRequest> Items { get; set; } = new();
    }

    public class CreateOrderItemRequest
    {
        public int ProductId { get; set; }

        public int Quantity { get; set; }
    }
}
