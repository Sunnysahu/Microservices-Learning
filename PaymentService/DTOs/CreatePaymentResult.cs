namespace PaymentService.DTOs
{
    public enum CreatePaymentStatus
    {
        Success,
        IdempotencyConflict
    }

    public class CreatePaymentResult
    {
        public CreatePaymentStatus Status { get; set; }

        public string Message { get; set; } = string.Empty;

        public PaymentResponse? Payment { get; set; }
    }
}
