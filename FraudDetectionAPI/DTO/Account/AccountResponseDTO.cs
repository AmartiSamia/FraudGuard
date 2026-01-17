namespace FraudDetectionAPI.DTO.Account
{
    public class AccountResponseDTO
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string AccountNumber { get; set; } = string.Empty;
        public decimal Balance { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
