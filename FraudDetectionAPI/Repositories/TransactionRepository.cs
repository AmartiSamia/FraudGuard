using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FraudDetectionAPI.Data;
using FraudDetectionAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace FraudDetectionAPI.Repositories
{
    public class TransactionRepository : ITransactionRepository
    {
        private readonly ApplicationDbContext _db;

        public TransactionRepository(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task AddAsync(Transaction transaction)
        {
            await _db.Transactions.AddAsync(transaction);
        }

        public async Task<IEnumerable<Transaction>> GetAllAsync()
        {
            return await _db.Transactions.ToListAsync();
        }

        public async Task<IEnumerable<Transaction>> GetByAccountIdAsync(int accountId)
        {
            return await _db.Transactions.Where(u => u.AccountId == accountId).OrderByDescending(t => t.Timestamp).ToListAsync();
        }

        public async Task<Transaction?> GetByIdAsync(int id)
        {
            return await _db.Transactions.Include(t => t.Account).FirstOrDefaultAsync(u=>u.Id == id);
        }

        public async Task<IEnumerable<Transaction>> GetFraudulentAsync()
        {
            return await _db.Transactions.Where(t => t.IsFraud).ToListAsync();
        }

        public async Task SaveChangesAsync()
        {
            await _db.SaveChangesAsync();
        }
    }
}
