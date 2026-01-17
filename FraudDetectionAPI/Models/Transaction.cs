namespace FraudDetectionAPI.Models
{
    public class Transaction
    {
        public int Id { get; set; }

        public int AccountId { get; set; }
        public Account Account { get; set; } = null!;

        public decimal Amount { get; set; }
        public string Type { get; set; } = string.Empty;   // Virement, Retrait, Depot, Prelevement, etc.
        public string Country { get; set; } = string.Empty; // "MA", "FR", etc.
        public string Device { get; set; } = string.Empty;  // "Web", "Mobile", "ATM", "Agency", "POS"

        // RIB du destinataire pour les virements
        public string? RecipientRIB { get; set; }
        
        // Description de la transaction
        public string? Description { get; set; }

        public DateTime Timestamp { get; set; } = DateTime.UtcNow;

        public bool IsFraud { get; set; }
        public string? FraudReason { get; set; }

        // Navigation property - one-to-one with FraudAlert
        public FraudAlert? FraudAlert { get; set; }
    }
}
