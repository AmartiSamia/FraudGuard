using FraudDetectionAPI.Models;

namespace FraudDetectionAPI.Services
{
    public interface ITransactionService
    {
        Task<Transaction> CreateTransactionAsync(Transaction transaction);
        Task<IEnumerable<Transaction>> GetByAccountIdAsync(int accountId);
        Task<IEnumerable<Transaction>> GetFraudulentAsync();
        Task<IEnumerable<Transaction>> GetAllAsync();
    }
}
