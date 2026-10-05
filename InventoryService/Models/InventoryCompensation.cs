namespace InventoryService.Models
{
    public class InventoryCompensation
    {
        public int Id { get; set; }

        public int OrderId { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
