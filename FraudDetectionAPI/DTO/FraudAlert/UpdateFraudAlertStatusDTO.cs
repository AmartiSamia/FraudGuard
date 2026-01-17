namespace FraudDetectionAPI.DTO.FraudAlert
{
    public class UpdateFraudAlertStatusDTO
    {
        public string Status { get; set; } = string.Empty; // "New", "Confirmed", "Ignored"
    }
}
