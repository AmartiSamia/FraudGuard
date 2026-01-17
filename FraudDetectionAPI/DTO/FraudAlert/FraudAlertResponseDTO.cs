namespace FraudDetectionAPI.DTO.FraudAlert
{
    public class FraudAlertResponseDTO
    {
        public int Id { get; set; }

        public int TransactionId { get; set; }

        public double RiskScore { get; set; }

        public string Reason { get; set; } = string.Empty;

        public string Status { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; }
    }
}
