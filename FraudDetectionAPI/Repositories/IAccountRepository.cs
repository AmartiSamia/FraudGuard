using FraudDetectionAPI.Models;

namespace FraudDetectionAPI.Repositories
{
    public interface IAccountRepository
    {
        Task<Account?> GetByUserIdAsync(int userId);
        Task AddAsync(Account account);
        Task<Account?> GetByIdAsync(int id);
        Task SaveChangesAsync();
    }
}
