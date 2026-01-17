using FraudDetectionAPI.Data;
using FraudDetectionAPI.DTO.User;
using FraudDetectionAPI.Models;
using FraudDetectionAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FraudDetectionAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin")]
    public class AdminController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IUserService _userService;

        public AdminController(ApplicationDbContext context, IUserService userService)
        {
            _context = context;
            _userService = userService;
        }

        // ============================================
        // USER MANAGEMENT
        // ============================================

        /// <summary>
        /// Get all users with pagination and filtering
        /// </summary>
        [HttpGet("users")]
        public async Task<ActionResult> GetUsers(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20,
            [FromQuery] string? role = null,
            [FromQuery] string? search = null)
        {
            try
            {
                var query = _context.Users
                    .Include(u => u.Accounts)
                        .ThenInclude(a => a.Transactions)
                    .AsQueryable();

                if (!string.IsNullOrEmpty(role))
                    query = query.Where(u => u.Role == role);

                if (!string.IsNullOrEmpty(search))
                    query = query.Where(u => 
                        u.Email.Contains(search) || 
                        u.FirstName.Contains(search) || 
                        u.LastName.Contains(search));

                var totalCount = await query.CountAsync();
                var users = await query
                    .OrderByDescending(u => u.CreatedAt)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                var result = users.Select(u => new {
                    u.Id,
                    u.FirstName,
                    u.LastName,
                    u.Email,
                    u.Role,
                    u.CreatedAt,
                    AccountCount = u.Accounts?.Count ?? 0,
                    TransactionCount = u.Accounts?.SelectMany(a => a.Transactions ?? new List<Transaction>()).Count() ?? 0
                }).ToList();

                return Ok(new {
                    data = result,
                    pagination = new {
                        currentPage = page,
                        pageSize,
                        totalCount,
                        totalPages = (int)Math.Ceiling((double)totalCount / pageSize)
                    }
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error loading users", error = ex.Message });
            }
        }

        /// <summary>
        /// Get user by ID with full details
        /// </summary>
        [HttpGet("users/{id}")]
        public async Task<ActionResult> GetUser(int id)
        {
            var user = await _context.Users
                .Include(u => u.Accounts)
                    .ThenInclude(a => a.Transactions)
                .FirstOrDefaultAsync(u => u.Id == id);

            if (user == null) return NotFound(new { message = "User not found" });

            var stats = new {
                totalTransactions = user.Accounts.SelectMany(a => a.Transactions).Count(),
                fraudTransactions = user.Accounts.SelectMany(a => a.Transactions).Count(t => t.IsFraud),
                totalAmount = user.Accounts.SelectMany(a => a.Transactions).Sum(t => t.Amount),
                totalBalance = user.Accounts.Sum(a => a.Balance)
            };

            return Ok(new {
                user = new {
                    user.Id,
                    user.FirstName,
                    user.LastName,
                    user.Email,
                    user.Role,
                    user.CreatedAt,
                    accounts = user.Accounts.Select(a => new {
                        a.Id,
                        a.AccountNumber,
                        a.Balance,
                        a.CreatedAt
                    })
                },
                stats
            });
        }

        /// <summary>
        /// Create a new user (admin can create users/admins)
        /// </summary>
        [HttpPost("users")]
        public async Task<ActionResult> CreateUser([FromBody] CreateUserRequest request)
        {
            // Check if email exists
            var existing = await _context.Users.FirstOrDefaultAsync(u => u.Email == request.Email);
            if (existing != null) return BadRequest(new { message = "Email already exists" });

            var user = new User
            {
                FirstName = request.FirstName,
                LastName = request.LastName,
                Email = request.Email,
                Password = BCrypt.Net.BCrypt.HashPassword(request.Password),
                Role = request.Role ?? "User",
                CreatedAt = DateTime.UtcNow
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            // Create default account
            if (request.CreateDefaultAccount)
            {
                var account = new Account
                {
                    UserId = user.Id,
                    AccountNumber = $"FG{user.Id:D8}",
                    Balance = request.InitialBalance ?? 1000m,
                    CreatedAt = DateTime.UtcNow
                };
                _context.Accounts.Add(account);
                await _context.SaveChangesAsync();
            }

            return Ok(new { message = "User created successfully", userId = user.Id });
        }

        /// <summary>
        /// Update user details
        /// </summary>
        [HttpPut("users/{id}")]
        public async Task<ActionResult> UpdateUser(int id, [FromBody] UpdateUserRequest request)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null) return NotFound(new { message = "User not found" });

            if (!string.IsNullOrEmpty(request.FirstName)) user.FirstName = request.FirstName;
            if (!string.IsNullOrEmpty(request.LastName)) user.LastName = request.LastName;
            if (!string.IsNullOrEmpty(request.Email))
            {
                var existing = await _context.Users.FirstOrDefaultAsync(u => u.Email == request.Email && u.Id != id);
                if (existing != null) return BadRequest(new { message = "Email already in use" });
                user.Email = request.Email;
            }
            if (!string.IsNullOrEmpty(request.Role)) user.Role = request.Role;
            if (!string.IsNullOrEmpty(request.Password))
            {
                user.Password = BCrypt.Net.BCrypt.HashPassword(request.Password);
            }

            await _context.SaveChangesAsync();
            return Ok(new { message = "User updated successfully" });
        }

        /// <summary>
        /// Delete user and all associated data
        /// </summary>
        [HttpDelete("users/{id}")]
        public async Task<ActionResult> DeleteUser(int id)
        {
            var user = await _context.Users
                .Include(u => u.Accounts)
                    .ThenInclude(a => a.Transactions)
                .FirstOrDefaultAsync(u => u.Id == id);

            if (user == null) return NotFound(new { message = "User not found" });

            // Don't allow deleting yourself
            var currentUserId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? "0");
            if (user.Id == currentUserId)
                return BadRequest(new { message = "Cannot delete your own account" });

            // Delete related fraud alerts
            var transactionIds = user.Accounts.SelectMany(a => a.Transactions).Select(t => t.Id).ToList();
            var alerts = await _context.FraudAlerts.Where(f => transactionIds.Contains(f.TransactionId)).ToListAsync();
            _context.FraudAlerts.RemoveRange(alerts);

            // Delete transactions
            foreach (var account in user.Accounts)
            {
                _context.Transactions.RemoveRange(account.Transactions);
            }

            // Delete accounts
            _context.Accounts.RemoveRange(user.Accounts);

            // Delete user
            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            return Ok(new { message = "User deleted successfully" });
        }

        // ============================================
        // TRANSACTION MANAGEMENT
        // ============================================

        /// <summary>
        /// Get all transactions with pagination and filtering
        /// </summary>
        [HttpGet("transactions")]
        public async Task<ActionResult> GetTransactions(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 50,
            [FromQuery] bool? isFraud = null,
            [FromQuery] string? country = null,
            [FromQuery] string? type = null,
            [FromQuery] decimal? minAmount = null,
            [FromQuery] decimal? maxAmount = null,
            [FromQuery] DateTime? startDate = null,
            [FromQuery] DateTime? endDate = null)
        {
            var query = _context.Transactions
                .Include(t => t.Account)
                    .ThenInclude(a => a.User)
                .AsQueryable();

            if (isFraud.HasValue)
                query = query.Where(t => t.IsFraud == isFraud.Value);
            if (!string.IsNullOrEmpty(country))
                query = query.Where(t => t.Country == country);
            if (!string.IsNullOrEmpty(type))
                query = query.Where(t => t.Type == type);
            if (minAmount.HasValue)
                query = query.Where(t => t.Amount >= minAmount.Value);
            if (maxAmount.HasValue)
                query = query.Where(t => t.Amount <= maxAmount.Value);
            if (startDate.HasValue)
                query = query.Where(t => t.Timestamp >= startDate.Value);
            if (endDate.HasValue)
                query = query.Where(t => t.Timestamp <= endDate.Value);

            var totalCount = await query.CountAsync();
            var transactions = await query
                .OrderByDescending(t => t.Timestamp)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(t => new {
                    t.Id,
                    t.AccountId,
                    accountNumber = t.Account.AccountNumber,
                    userEmail = t.Account.User.Email,
                    userName = t.Account.User.FirstName + " " + t.Account.User.LastName,
                    t.Amount,
                    t.Type,
                    t.Country,
                    t.Device,
                    t.Timestamp,
                    t.IsFraud,
                    t.FraudReason
                })
                .ToListAsync();

            return Ok(new {
                data = transactions,
                pagination = new {
                    currentPage = page,
                    pageSize,
                    totalCount,
                    totalPages = (int)Math.Ceiling((double)totalCount / pageSize)
                }
            });
        }

        /// <summary>
        /// Get transaction details with full context
        /// </summary>
        [HttpGet("transactions/{id}")]
        public async Task<ActionResult> GetTransaction(int id)
        {
            var transaction = await _context.Transactions
                .Include(t => t.Account)
                    .ThenInclude(a => a.User)
                .Include(t => t.FraudAlert)
                .FirstOrDefaultAsync(t => t.Id == id);

            if (transaction == null) return NotFound(new { message = "Transaction not found" });

            // Get related transactions for context
            var relatedTransactions = await _context.Transactions
                .Where(t => t.AccountId == transaction.AccountId && t.Id != id)
                .OrderByDescending(t => t.Timestamp)
                .Take(10)
                .Select(t => new { t.Id, t.Amount, t.Type, t.Timestamp, t.IsFraud })
                .ToListAsync();

            return Ok(new {
                transaction = new {
                    transaction.Id,
                    transaction.AccountId,
                    accountNumber = transaction.Account.AccountNumber,
                    accountBalance = transaction.Account.Balance,
                    user = new {
                        transaction.Account.User.Id,
                        transaction.Account.User.Email,
                        name = transaction.Account.User.FirstName + " " + transaction.Account.User.LastName
                    },
                    transaction.Amount,
                    transaction.Type,
                    transaction.Country,
                    transaction.Device,
                    transaction.Timestamp,
                    transaction.IsFraud,
                    transaction.FraudReason,
                    fraudAlert = transaction.FraudAlert != null ? new {
                        transaction.FraudAlert.Id,
                        transaction.FraudAlert.RiskScore,
                        transaction.FraudAlert.Status,
                        transaction.FraudAlert.CreatedAt,
                        transaction.FraudAlert.UpdatedAt
                    } : null
                },
                relatedTransactions
            });
        }

        /// <summary>
        /// Update transaction (mark as fraud/not fraud)
        /// </summary>
        [HttpPut("transactions/{id}")]
        public async Task<ActionResult> UpdateTransaction(int id, [FromBody] UpdateTransactionRequest request)
        {
            var transaction = await _context.Transactions.FindAsync(id);
            if (transaction == null) return NotFound(new { message = "Transaction not found" });

            if (request.IsFraud.HasValue) transaction.IsFraud = request.IsFraud.Value;
            if (!string.IsNullOrEmpty(request.FraudReason)) transaction.FraudReason = request.FraudReason;

            await _context.SaveChangesAsync();
            return Ok(new { message = "Transaction updated successfully" });
        }

        /// <summary>
        /// Delete transaction
        /// </summary>
        [HttpDelete("transactions/{id}")]
        public async Task<ActionResult> DeleteTransaction(int id)
        {
            var transaction = await _context.Transactions
                .Include(t => t.FraudAlert)
                .FirstOrDefaultAsync(t => t.Id == id);

            if (transaction == null) return NotFound(new { message = "Transaction not found" });

            // Delete fraud alert if exists
            if (transaction.FraudAlert != null)
            {
                _context.FraudAlerts.Remove(transaction.FraudAlert);
            }

            _context.Transactions.Remove(transaction);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Transaction deleted successfully" });
        }

        // ============================================
        // ANALYTICS & REPORTS
        // ============================================

        /// <summary>
        /// Get comprehensive analytics
        /// </summary>
        [HttpGet("analytics")]
        public async Task<ActionResult> GetAnalytics()
        {
            var now = DateTime.UtcNow;
            var last30Days = now.AddDays(-30);
            var last7Days = now.AddDays(-7);

            // Overall stats
            var totalUsers = await _context.Users.CountAsync();
            var totalTransactions = await _context.Transactions.CountAsync();
            var totalFraud = await _context.Transactions.CountAsync(t => t.IsFraud);
            var totalAmount = await _context.Transactions.SumAsync(t => t.Amount);

            // 30-day stats
            var trans30 = await _context.Transactions.Where(t => t.Timestamp >= last30Days).ToListAsync();
            var fraud30 = trans30.Count(t => t.IsFraud);

            // 7-day stats
            var trans7 = trans30.Where(t => t.Timestamp >= last7Days).ToList();
            var fraud7 = trans7.Count(t => t.IsFraud);

            // Top countries by fraud
            var fraudByCountry = await _context.Transactions
                .Where(t => t.IsFraud)
                .GroupBy(t => t.Country)
                .Select(g => new { country = g.Key, count = g.Count(), amount = g.Sum(t => t.Amount) })
                .OrderByDescending(x => x.count)
                .Take(10)
                .ToListAsync();

            // Top devices by fraud
            var fraudByDevice = await _context.Transactions
                .Where(t => t.IsFraud)
                .GroupBy(t => t.Device)
                .Select(g => new { device = g.Key, count = g.Count() })
                .OrderByDescending(x => x.count)
                .ToListAsync();

            // Daily trend (last 30 days)
            var dailyTrend = trans30
                .GroupBy(t => t.Timestamp.Date)
                .Select(g => new {
                    date = g.Key,
                    total = g.Count(),
                    fraud = g.Count(t => t.IsFraud),
                    amount = g.Sum(t => t.Amount)
                })
                .OrderBy(x => x.date)
                .ToList();

            // High risk users
            var highRiskUsers = await _context.Users
                .Select(u => new {
                    u.Id,
                    u.Email,
                    name = u.FirstName + " " + u.LastName,
                    fraudCount = u.Accounts.SelectMany(a => a.Transactions).Count(t => t.IsFraud),
                    totalTrans = u.Accounts.SelectMany(a => a.Transactions).Count()
                })
                .Where(u => u.fraudCount > 0)
                .OrderByDescending(u => u.fraudCount)
                .Take(10)
                .ToListAsync();

            return Ok(new {
                overview = new {
                    totalUsers,
                    totalTransactions,
                    totalFraud,
                    fraudRate = totalTransactions > 0 ? Math.Round((double)totalFraud / totalTransactions * 100, 2) : 0,
                    totalAmount
                },
                last30Days = new {
                    transactions = trans30.Count,
                    fraud = fraud30,
                    fraudRate = trans30.Count > 0 ? Math.Round((double)fraud30 / trans30.Count * 100, 2) : 0
                },
                last7Days = new {
                    transactions = trans7.Count,
                    fraud = fraud7,
                    fraudRate = trans7.Count > 0 ? Math.Round((double)fraud7 / trans7.Count * 100, 2) : 0
                },
                fraudByCountry,
                fraudByDevice,
                dailyTrend,
                highRiskUsers
            });
        }

        /// <summary>
        /// Export data (for Power BI / Excel)
        /// </summary>
        [HttpGet("export/{type}")]
        public async Task<ActionResult> ExportData(string type, [FromQuery] DateTime? startDate, [FromQuery] DateTime? endDate)
        {
            startDate ??= DateTime.UtcNow.AddMonths(-1);
            endDate ??= DateTime.UtcNow;

            switch (type.ToLower())
            {
                case "transactions":
                    var transactions = await _context.Transactions
                        .Include(t => t.Account).ThenInclude(a => a.User)
                        .Where(t => t.Timestamp >= startDate && t.Timestamp <= endDate)
                        .Select(t => new {
                            t.Id,
                            UserEmail = t.Account.User.Email,
                            t.Account.AccountNumber,
                            t.Amount,
                            t.Type,
                            t.Country,
                            t.Device,
                            t.Timestamp,
                            t.IsFraud,
                            t.FraudReason
                        })
                        .ToListAsync();
                    return Ok(transactions);

                case "users":
                    var users = await _context.Users
                        .Select(u => new {
                            u.Id,
                            u.Email,
                            u.FirstName,
                            u.LastName,
                            u.Role,
                            u.CreatedAt,
                            AccountCount = u.Accounts.Count,
                            TotalTransactions = u.Accounts.SelectMany(a => a.Transactions).Count(),
                            FraudTransactions = u.Accounts.SelectMany(a => a.Transactions).Count(t => t.IsFraud)
                        })
                        .ToListAsync();
                    return Ok(users);

                case "fraud-summary":
                    var summary = await _context.Transactions
                        .Where(t => t.IsFraud && t.Timestamp >= startDate && t.Timestamp <= endDate)
                        .GroupBy(t => new { t.Country, t.Device, t.Type })
                        .Select(g => new {
                            g.Key.Country,
                            g.Key.Device,
                            g.Key.Type,
                            Count = g.Count(),
                            TotalAmount = g.Sum(t => t.Amount),
                            AvgAmount = g.Average(t => t.Amount)
                        })
                        .OrderByDescending(x => x.Count)
                        .ToListAsync();
                    return Ok(summary);

                default:
                    return BadRequest(new { message = "Invalid export type" });
            }
        }
    }

    // Request DTOs
    public class CreateUserRequest
    {
        public string FirstName { get; set; } = "";
        public string LastName { get; set; } = "";
        public string Email { get; set; } = "";
        public string Password { get; set; } = "";
        public string? Role { get; set; }
        public bool CreateDefaultAccount { get; set; } = true;
        public decimal? InitialBalance { get; set; }
    }

    public class UpdateUserRequest
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Email { get; set; }
        public string? Password { get; set; }
        public string? Role { get; set; }
    }

    public class UpdateTransactionRequest
    {
        public bool? IsFraud { get; set; }
        public string? FraudReason { get; set; }
    }
}
