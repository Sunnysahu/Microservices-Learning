namespace InventoryService.DTOs
{
    public class ReserveStockBatchRequest
    {
        public List<ReserveStockItemRequest> Items { get; set; } = new();
    }

    public class ReserveStockItemRequest
    {
        public int ProductId { get; set; }
        public int Quantity { get; set; }
    }
}
