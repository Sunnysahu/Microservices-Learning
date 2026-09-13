using InventoryService.DTOs;
using InventoryService.Services;
using Microsoft.AspNetCore.Mvc;

namespace InventoryService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InventoryController : ControllerBase
    {
        private readonly IInventoryService _inventoryService;

        public InventoryController(IInventoryService inventoryService)
        {
            _inventoryService = inventoryService;
        }

        [HttpGet]
        public async Task<ActionResult<List<InventoryResponse>>> GetAll(CancellationToken cancellationToken)
        {
            var items = await _inventoryService.GetAllAsync(cancellationToken);

            var response = items.Select(item => new InventoryResponse
            {
                ProductId = item.ProductId,
                StockQuantity = item.StockQuantity
            }).ToList();

            return Ok(response);
        }

        [HttpGet("{productId:int}")]
        public async Task<ActionResult<InventoryResponse>> GetByProductId(int productId, CancellationToken cancellationToken)
        {
            var item = await _inventoryService.GetByProductIdAsync(productId, cancellationToken);

            if (item is null)
            {
                return NotFound();
            }

            var response = new InventoryResponse
            {
                ProductId = item.ProductId,
                StockQuantity = item.StockQuantity
            };

            return Ok(response);
        }
    }
}
