using FraudDetectionAPI.Models;

namespace FraudDetectionAPI.Services
{
    public interface IAccountService
    {
        Task<Account> CreateAccountAsync(int userId, decimal initialBalance);
        Task<Account?> GetAccountByUserIdAsync(int userId);
    }
}
