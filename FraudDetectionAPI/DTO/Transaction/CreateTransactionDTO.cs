namespace FraudDetectionAPI.DTO.Transaction
{
    public class CreateTransactionDTO
    {
        public int AccountId { get; set; }
        public decimal Amount { get; set; }
        public string Type { get; set; } = string.Empty;  // Virement, Retrait, Depot, etc.
        public string Country { get; set; } = string.Empty;
        public string Device { get; set; } = string.Empty;
        public string? RecipientRIB { get; set; }  // RIB du destinataire pour les virements
        public string? Description { get; set; }   // Description optionnelle
    }
}
