using FraudDetectionAPI.Models;
using System;
using System.Collections.Generic;

namespace FraudDetectionAPI.Data
{
    public class DatabaseSeeder
    {
        private static readonly Random _random = new Random(42); // Fixed seed for reproducibility
        private static readonly string[] _countries = { "US", "UK", "FR", "DE", "CA", "AU", "JP", "BR", "MX", "IN", "CN", "RU", "NG", "KE" };
        private static readonly string[] _devices = { "mobile", "web", "atm", "pos", "tablet" };
        private static readonly string[] _transactionTypes = { "debit", "credit", "transfer", "withdrawal", "deposit", "payment" };
        private static readonly string[] _firstNames = { "James", "Mary", "John", "Patricia", "Robert", "Jennifer", "Michael", "Linda", "William", "Elizabeth", "David", "Barbara", "Richard", "Susan", "Joseph", "Jessica", "Thomas", "Sarah", "Charles", "Karen" };
        private static readonly string[] _lastNames = { "Smith", "Johnson", "Williams", "Brown", "Jones", "Garcia", "Miller", "Davis", "Rodriguez", "Martinez", "Hernandez", "Lopez", "Gonzalez", "Wilson", "Anderson", "Thomas", "Taylor", "Moore", "Jackson", "Martin" };

        public static void SeedDatabase(ApplicationDbContext context)
        {
            try
            {
                // Check if database already seeded
                if (context.Users.Any())
                {
                    Console.WriteLine("✅ Database already seeded - skipping");
                    return;
                }

                Console.WriteLine("🌱 Starting comprehensive database seeding...");

                // ===========================
                // USERS - Create diverse user base
                // ===========================
                var users = new List<User>();

                // Admin users
                users.Add(new User
                {
                    Email = "admin@fraudguard.com",
                    Password = BCrypt.Net.BCrypt.HashPassword("Admin@123", workFactor: 12),
                    FirstName = "System",
                    LastName = "Administrator",
                    Role = "Admin",
                    CreatedAt = DateTime.UtcNow.AddDays(-_random.Next(30, 365))
                });

                users.Add(new User
                {
                    Email = "security@fraudguard.com",
                    Password = BCrypt.Net.BCrypt.HashPassword("Security@123", workFactor: 12),
                    FirstName = "Security",
                    LastName = "Officer",
                    Role = "Admin",
                    CreatedAt = DateTime.UtcNow.AddDays(-_random.Next(30, 365))
                });

                users.Add(new User
                {
                    Email = "analyst@fraudguard.com",
                    Password = BCrypt.Net.BCrypt.HashPassword("Analyst@123", workFactor: 12),
                    FirstName = "Data",
                    LastName = "Analyst",
                    Role = "Admin",
                    CreatedAt = DateTime.UtcNow.AddDays(-_random.Next(30, 365))
                });

                // Regular users - 20 diverse users
                for (int i = 0; i < 20; i++)
                {
                    var firstName = _firstNames[i % _firstNames.Length];
                    var lastName = _lastNames[i % _lastNames.Length];
                    users.Add(new User
                    {
                        Email = $"{firstName.ToLower()}.{lastName.ToLower()}{i}@email.com",
                        Password = BCrypt.Net.BCrypt.HashPassword($"User{i}@123", workFactor: 12),
                        FirstName = firstName,
                        LastName = lastName,
                        Role = "User",
                        CreatedAt = DateTime.UtcNow.AddDays(-_random.Next(1, 180))
                    });
                }

                // Demo user for easy testing
                users.Add(new User
                {
                    Email = "demo@test.com",
                    Password = BCrypt.Net.BCrypt.HashPassword("demo123", workFactor: 12),
                    FirstName = "Demo",
                    LastName = "User",
                    Role = "User",
                    CreatedAt = DateTime.UtcNow.AddDays(-7)
                });

                context.Users.AddRange(users);
                context.SaveChanges();
                Console.WriteLine($"   ✓ Created {users.Count} users");

                // ===========================
                // ACCOUNTS - Multiple accounts per user
                // ===========================
                var accounts = new List<Account>();
                var accountIndex = 1;

                foreach (var user in users)
                {
                    // Each user gets 1-3 accounts
                    int numAccounts = user.Role == "Admin" ? 1 : _random.Next(1, 4);
                    
                    for (int i = 0; i < numAccounts; i++)
                    {
                        var accountType = i == 0 ? "Checking" : (i == 1 ? "Savings" : "Investment");
                        accounts.Add(new Account
                        {
                            UserId = user.Id,
                            AccountNumber = $"FG{accountIndex:D8}",
                            Balance = GenerateRealisticBalance(accountType),
                            CreatedAt = DateTime.UtcNow.AddDays(-_random.Next(30, 730))
                        });
                        accountIndex++;
                    }
                }

                context.Accounts.AddRange(accounts);
                context.SaveChanges();
                Console.WriteLine($"   ✓ Created {accounts.Count} accounts");

                // ===========================
                // TRANSACTIONS - Realistic transaction history
                // ===========================
                var transactions = new List<Transaction>();
                var fraudAlerts = new List<FraudAlert>();

                foreach (var account in accounts)
                {
                    // Generate 20-100 transactions per account over last 6 months
                    int numTransactions = _random.Next(20, 101);
                    
                    for (int i = 0; i < numTransactions; i++)
                    {
                        var (transaction, isSuspicious) = GenerateRealisticTransaction(account, i);
                        transactions.Add(transaction);
                    }
                }

                context.Transactions.AddRange(transactions);
                context.SaveChanges();
                Console.WriteLine($"   ✓ Created {transactions.Count} transactions");

                // ===========================
                // FRAUD ALERTS - Based on suspicious transactions
                // ===========================
                var suspiciousTransactions = transactions.Where(t => t.IsFraud).ToList();
                
                foreach (var tx in suspiciousTransactions)
                {
                    var alert = new FraudAlert
                    {
                        TransactionId = tx.Id,
                        RiskScore = GenerateRiskScore(tx),
                        Status = GenerateAlertStatus(),
                        CreatedAt = tx.Timestamp.AddMinutes(_random.Next(1, 60)),
                        UpdatedAt = tx.Timestamp.AddHours(_random.Next(1, 48))
                    };
                    fraudAlerts.Add(alert);
                }

                // Add some additional alerts for edge cases
                var normalTransactions = transactions.Where(t => !t.IsFraud).OrderByDescending(t => t.Amount).Take(15).ToList();
                foreach (var tx in normalTransactions)
                {
                    if (_random.NextDouble() > 0.5)
                    {
                        var alert = new FraudAlert
                        {
                            TransactionId = tx.Id,
                            RiskScore = 0.3 + (_random.NextDouble() * 0.3), // Lower risk scores for false positives
                            Status = "Resolved",
                            CreatedAt = tx.Timestamp.AddMinutes(_random.Next(1, 60)),
                            UpdatedAt = tx.Timestamp.AddHours(_random.Next(1, 24))
                        };
                        fraudAlerts.Add(alert);
                    }
                }

                context.FraudAlerts.AddRange(fraudAlerts);
                context.SaveChanges();
                Console.WriteLine($"   ✓ Created {fraudAlerts.Count} fraud alerts");

                // ===========================
                // SUMMARY
                // ===========================
                Console.WriteLine("\n📊 Database Seeding Summary:");
                Console.WriteLine($"   • Users: {users.Count} (Admin: {users.Count(u => u.Role == "Admin")}, Regular: {users.Count(u => u.Role == "User")})");
                Console.WriteLine($"   • Accounts: {accounts.Count}");
                Console.WriteLine($"   • Transactions: {transactions.Count} (Suspicious: {transactions.Count(t => t.IsFraud)})");
                Console.WriteLine($"   • Fraud Alerts: {fraudAlerts.Count} (Pending: {fraudAlerts.Count(a => a.Status == "Pending")})");
                Console.WriteLine("\n🔐 Test Credentials:");
                Console.WriteLine("   Admin: admin@fraudguard.com / Admin@123");
                Console.WriteLine("   User:  demo@test.com / demo123");
                Console.WriteLine("\n✅ Database seeding completed successfully!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Seeding error: {ex.Message}");
                Console.WriteLine($"   Stack: {ex.StackTrace}");
            }
        }

        private static decimal GenerateRealisticBalance(string accountType)
        {
            return accountType switch
            {
                "Checking" => Math.Round((decimal)(_random.NextDouble() * 15000 + 500), 2),
                "Savings" => Math.Round((decimal)(_random.NextDouble() * 50000 + 1000), 2),
                "Investment" => Math.Round((decimal)(_random.NextDouble() * 100000 + 5000), 2),
                _ => Math.Round((decimal)(_random.NextDouble() * 10000 + 100), 2)
            };
        }

        private static (Transaction, bool) GenerateRealisticTransaction(Account account, int index)
        {
            var daysAgo = _random.Next(0, 180);
            var timestamp = DateTime.UtcNow.AddDays(-daysAgo).AddHours(_random.Next(0, 24)).AddMinutes(_random.Next(0, 60));
            
            // Determine if this should be a suspicious transaction (about 8% fraud rate)
            bool isSuspicious = _random.NextDouble() < 0.08;
            
            var amount = isSuspicious 
                ? GenerateSuspiciousAmount() 
                : GenerateNormalAmount();

            var country = isSuspicious && _random.NextDouble() < 0.6
                ? _countries[_random.Next(10, _countries.Length)] // Higher risk countries
                : _countries[_random.Next(0, 6)]; // Normal countries

            var device = _devices[_random.Next(_devices.Length)];
            var type = _transactionTypes[_random.Next(_transactionTypes.Length)];

            // Suspicious patterns
            if (isSuspicious)
            {
                // Late night transactions are more suspicious
                timestamp = timestamp.Date.AddHours(_random.Next(1, 5));
                
                // Multiple transactions in short time
                if (_random.NextDouble() < 0.3)
                {
                    device = "web";
                    type = "transfer";
                }
            }

            var transaction = new Transaction
            {
                AccountId = account.Id,
                Amount = amount,
                Type = type,
                Country = country,
                Device = device,
                Timestamp = timestamp,
                IsFraud = isSuspicious
            };

            return (transaction, isSuspicious);
        }

        private static decimal GenerateNormalAmount()
        {
            // Normal distribution of transaction amounts
            var rand = _random.NextDouble();
            
            if (rand < 0.4) // 40% small transactions
                return Math.Round((decimal)(_random.NextDouble() * 100 + 5), 2);
            else if (rand < 0.7) // 30% medium transactions
                return Math.Round((decimal)(_random.NextDouble() * 500 + 100), 2);
            else if (rand < 0.9) // 20% larger transactions
                return Math.Round((decimal)(_random.NextDouble() * 2000 + 500), 2);
            else // 10% large transactions
                return Math.Round((decimal)(_random.NextDouble() * 5000 + 2000), 2);
        }

        private static decimal GenerateSuspiciousAmount()
        {
            // Suspicious transactions tend to be larger
            var rand = _random.NextDouble();
            
            if (rand < 0.3) // 30% very large
                return Math.Round((decimal)(_random.NextDouble() * 15000 + 5000), 2);
            else if (rand < 0.6) // 30% large
                return Math.Round((decimal)(_random.NextDouble() * 5000 + 2000), 2);
            else // 40% medium-large
                return Math.Round((decimal)(_random.NextDouble() * 2000 + 1000), 2);
        }

        private static double GenerateRiskScore(Transaction tx)
        {
            double baseScore = 0.5;
            
            // Higher amounts = higher risk
            if (tx.Amount > 5000) baseScore += 0.2;
            else if (tx.Amount > 2000) baseScore += 0.1;
            
            // High risk countries
            if (new[] { "RU", "NG", "KE", "CN" }.Contains(tx.Country))
                baseScore += 0.15;
            
            // Late night transactions
            if (tx.Timestamp.Hour >= 0 && tx.Timestamp.Hour <= 5)
                baseScore += 0.1;

            // Add some randomness
            baseScore += (_random.NextDouble() * 0.2 - 0.1);
            
            return Math.Min(0.99, Math.Max(0.4, baseScore));
        }

        private static string GenerateAlertStatus()
        {
            var rand = _random.NextDouble();
            if (rand < 0.5) return "Pending";
            if (rand < 0.8) return "Under Review";
            return "Resolved";
        }
    }
}
