using System.Collections.Generic;
using System.Threading.Tasks;
using FraudDetectionAPI.Models;  

namespace FraudDetectionAPI.Repositories
{
    public interface ITransactionRepository
    {
        Task AddAsync(Transaction transaction);
        Task<Transaction?> GetByIdAsync(int id);
        Task<IEnumerable<Transaction>> GetByAccountIdAsync(int accountId);
        Task<IEnumerable<Transaction>> GetFraudulentAsync();
        Task<IEnumerable<Transaction>> GetAllAsync();
        Task SaveChangesAsync();
    }
}
