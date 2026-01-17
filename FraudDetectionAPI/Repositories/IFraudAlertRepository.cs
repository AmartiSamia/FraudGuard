using FraudDetectionAPI.Models;

namespace FraudDetectionAPI.Repositories
{
    public interface IFraudAlertRepository
    {
        Task AddAsync(FraudAlert alert);
        Task<FraudAlert?> GetByIdAsync(int id);
        Task<IEnumerable<FraudAlert>> GetAllAsync();
        Task<IEnumerable<FraudAlert>> GetByStatusAsync(string status);
        Task SaveChangesAsync();
    }
}
