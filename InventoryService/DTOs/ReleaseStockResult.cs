namespace InventoryService.DTOs;

public enum ReleaseStockStatus
{
    Released,
    AlreadyProcessed,
    ProductNotFound
}

public class ReleaseStockResult
{
    public ReleaseStockStatus Status { get; set; }

    public string Message { get; set; } = string.Empty;
}
