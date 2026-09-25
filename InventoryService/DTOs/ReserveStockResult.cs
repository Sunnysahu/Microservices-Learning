namespace InventoryService.DTOs
{
    public enum ReserveStockStatus
    {
        Success,
        ProductNotFound,
        InsufficientStock
    }

    public class ReserveStockResult
    {
        public ReserveStockStatus Status { get; set; }
        public string Message { get; set; } = string.Empty;
    }
}
