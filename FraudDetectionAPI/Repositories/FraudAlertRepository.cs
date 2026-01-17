using FraudDetectionAPI.Data;
using FraudDetectionAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace FraudDetectionAPI.Repositories
{
    public class FraudAlertRepository : IFraudAlertRepository
    {
        private readonly ApplicationDbContext _db;

        public FraudAlertRepository(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task AddAsync(FraudAlert alert)
        {
            await _db.FraudAlerts.AddAsync(alert);
        }

        public async Task<FraudAlert?> GetByIdAsync(int id)
        {
            return await _db.FraudAlerts
                .Include(a => a.Transaction)
                .FirstOrDefaultAsync(a => a.Id == id);
        }

        public async Task<IEnumerable<FraudAlert>> GetAllAsync()
        {
            return await _db.FraudAlerts
                .OrderByDescending(a => a.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<FraudAlert>> GetByStatusAsync(string status)
        {
            return await _db.FraudAlerts
                .Where(a => a.Status == status)
                .OrderByDescending(a => a.CreatedAt)
                .ToListAsync();
        }

        public async Task SaveChangesAsync()
        {
            await _db.SaveChangesAsync();
        }
    }
}
