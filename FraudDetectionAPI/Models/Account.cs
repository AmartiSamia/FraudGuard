

namespace FraudDetectionAPI.Models
{
    public class Account
    {
        public int Id { get; set; }

        public int UserId { get; set; }
        public User User { get; set; }

        public string AccountNumber { get; set; } = string.Empty;

        public decimal Balance { get; set; } = 0;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();

    }
}
