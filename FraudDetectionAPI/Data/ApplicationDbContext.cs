using Microsoft.EntityFrameworkCore;
using FraudDetectionAPI.Models;


namespace FraudDetectionAPI.Data
{
    public class ApplicationDbContext : DbContext
    {
        // Constructeur
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // Table Users -> SQL Server créera la table Users
        public DbSet<User> Users { get; set; }
        public DbSet<Account> Accounts { get; set; }
        public DbSet<Transaction> Transactions { get; set; }
        public DbSet<FraudAlert> FraudAlerts { get; set; } = null!;
    }
}
