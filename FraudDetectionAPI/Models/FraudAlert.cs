namespace FraudDetectionAPI.Models
{
    public class FraudAlert
    {
        public int Id { get; set; }

        // Relation to the transaction
        public int TransactionId { get; set; }
        public Transaction Transaction { get; set; } = null!;

        // Risk score (0.0 to 1.0)
        public double RiskScore { get; set; }

        // Text reason (triggered rules, AI, etc.)
        public string Reason { get; set; } = string.Empty;

        // "Pending", "Under Review", "Resolved", "Dismissed"
        public string Status { get; set; } = "Pending";

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
    }
}
