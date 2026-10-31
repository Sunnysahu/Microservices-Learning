namespace OrderService.Models
{
    public class OutboxMessage
    {
        public long Id { get; set; }

        public string EventType { get; set; } = string.Empty;

        public string Payload { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; }

        public DateTime? ProcessedAt { get; set; }

        public int RetryCount { get; set; }

        public DateTime? FailedAt { get; set; }
    }
}
