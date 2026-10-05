namespace InventoryService.DTOs
{
    public class ReleaseStockRequest
    {
        public int OrderId {  get; set; }
        public List<ReleaseStockItemRequest> Items { get; set; } = new();
    }

    public class ReleaseStockItemRequest
    {
        public int ProductId { get; set; }
        public int Quantity { get; set; }
    }
}
