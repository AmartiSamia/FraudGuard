using FraudDetectionAPI.Data;
using FraudDetectionAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace FraudDetectionAPI.Repositories
{
    public class AccountRepository : IAccountRepository
    {
        private readonly ApplicationDbContext _db;

        public AccountRepository(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task AddAsync(Account account)
        {
            await _db.Accounts.AddAsync(account);
        }
        public async Task<Account?> GetByIdAsync(int id)
        {
            return await _db.Accounts
                .FirstOrDefaultAsync(a => a.Id == id);
        }
        public async Task<Account?> GetByUserIdAsync(int userId)
        {
            return await _db.Accounts.FirstOrDefaultAsync(u => u.UserId == userId);
        }
        
        public async Task SaveChangesAsync()
        {
            await _db.SaveChangesAsync();
        }
    }
}
