using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PaymentService.DTOs;
using PaymentService.Services;

namespace PaymentService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentController : ControllerBase
    {
        private readonly IPaymentService _paymentService;

        public PaymentController(IPaymentService paymentService) => _paymentService = paymentService;

        [HttpGet("order/{orderId:int}")]
        public async Task<ActionResult<PaymentResponse>> GetByOrderId(int orderId, CancellationToken cancellationToken)
        {
            var payment = await _paymentService.GetByOrderIdAsync(orderId, cancellationToken);

            if (payment is null)
            {
                return NotFound(new
                {
                    success = false,
                    message = $"Payment for Order {orderId} was not found."
                });
            }

            return Ok(payment);
        }
        
        [Route("payment")]
        [HttpPost]
        public async Task<ActionResult<PaymentResponse>> Create(CreatePaymentRequest request, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(request.IdempotencyKey))
            {
                return BadRequest(new
                {
                    success = false,
                    message = "IdempotencyKey is required."
                });
            }

            var result = await _paymentService.CreateAsync(request, cancellationToken);

            if (result.Status == CreatePaymentStatus.IdempotencyConflict)
            {
                return Conflict(new
                {
                    success = false,
                    message = result.Message
                });
            }

            return Ok(result.Payment);
        }
    }
}
