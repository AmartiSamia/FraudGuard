namespace FraudDetectionAPI.DTO.Transaction
{
    public class TransactionResponseDTO
    {
        public int Id { get; set; }
        public int AccountId { get; set; }
        public string? AccountNumber { get; set; }

        public decimal Amount { get; set; }
        public string Type { get; set; } = string.Empty;
        public string Country { get; set; } = string.Empty;
        public string Device { get; set; } = string.Empty;
        public string? RecipientRIB { get; set; }
        public string? Description { get; set; }

        public DateTime Timestamp { get; set; }

        public bool IsFraud { get; set; }
        public string? FraudReason { get; set; }
    }
}
