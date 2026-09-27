namespace OrderService.Services
{
    public enum InventoryReservationStatus
    {
        Success,
        ProductNotFound,
        InsufficientStock
    }

    public class InventoryReservationResult
    {
        public InventoryReservationStatus Status { get; set; }
        public string Message { get; set; } = string.Empty;
    }
}
