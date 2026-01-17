using FraudDetectionAPI.Models;

namespace FraudDetectionAPI.Services
{
    public interface IFraudAlertService
    {
        Task<IEnumerable<FraudAlert>> GetAllAsync();
        Task<IEnumerable<FraudAlert>> GetByStatusAsync(string status);
        Task<FraudAlert?> GetByIdAsync(int id);
        Task<FraudAlert> UpdateStatusAsync(int id, string newStatus);
    }
}
