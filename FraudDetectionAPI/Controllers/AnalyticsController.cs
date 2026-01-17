using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using FraudDetectionAPI.Data;
using Microsoft.EntityFrameworkCore;

namespace FraudDetectionAPI.Controllers
{
    /// <summary>
    /// Analytics controller for admin dashboard and reporting
    /// Provides comprehensive statistics and insights
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Policy = "AdminOnly")]
    public class AnalyticsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<AnalyticsController> _logger;

        public AnalyticsController(
            ApplicationDbContext context,
            ILogger<AnalyticsController> logger)
        {
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// Get comprehensive dashboard statistics
        /// </summary>
        [HttpGet("dashboard")]
        public async Task<IActionResult> GetDashboardStats()
        {
            try
            {
                var now = DateTime.UtcNow;
                var today = now.Date;
                var thisWeek = today.AddDays(-(int)today.DayOfWeek);
                var thisMonth = new DateTime(now.Year, now.Month, 1);

                // User statistics
                var totalUsers = await _context.Users.CountAsync();
                var adminUsers = await _context.Users.CountAsync(u => u.Role == "Admin");
                var regularUsers = totalUsers - adminUsers;

                // Account statistics
                var totalAccounts = await _context.Accounts.CountAsync();
                var totalBalance = await _context.Accounts.SumAsync(a => a.Balance);

                // Transaction statistics
                var totalTransactions = await _context.Transactions.CountAsync();
                var todayTransactions = await _context.Transactions
                    .CountAsync(t => t.Timestamp >= today);
                var weekTransactions = await _context.Transactions
                    .CountAsync(t => t.Timestamp >= thisWeek);
                var monthTransactions = await _context.Transactions
                    .CountAsync(t => t.Timestamp >= thisMonth);

                var totalTransactionAmount = await _context.Transactions.SumAsync(t => t.Amount);
                var avgTransactionAmount = totalTransactions > 0 
                    ? totalTransactionAmount / totalTransactions 
                    : 0;

                // Fraud statistics
                var totalFraudTransactions = await _context.Transactions
                    .CountAsync(t => t.IsFraud);
                var fraudRate = totalTransactions > 0 
                    ? (double)totalFraudTransactions / totalTransactions * 100 
                    : 0;

                var fraudAmount = await _context.Transactions
                    .Where(t => t.IsFraud)
                    .SumAsync(t => t.Amount);

                // Alert statistics
                var totalAlerts = await _context.FraudAlerts.CountAsync();
                var pendingAlerts = await _context.FraudAlerts
                    .CountAsync(a => a.Status == "Pending");
                var resolvedAlerts = await _context.FraudAlerts
                    .CountAsync(a => a.Status == "Resolved");

                var avgRiskScore = await _context.FraudAlerts
                    .AverageAsync(a => (double?)a.RiskScore) ?? 0;

                return Ok(new
                {
                    overview = new
                    {
                        totalUsers,
                        adminUsers,
                        regularUsers,
                        totalAccounts,
                        totalBalance
                    },
                    transactions = new
                    {
                        total = totalTransactions,
                        today = todayTransactions,
                        thisWeek = weekTransactions,
                        thisMonth = monthTransactions,
                        totalAmount = totalTransactionAmount,
                        averageAmount = Math.Round(avgTransactionAmount, 2)
                    },
                    fraud = new
                    {
                        totalFraudTransactions,
                        fraudRate = Math.Round(fraudRate, 2),
                        fraudAmount,
                        preventedLoss = fraudAmount
                    },
                    alerts = new
                    {
                        total = totalAlerts,
                        pending = pendingAlerts,
                        resolved = resolvedAlerts,
                        underReview = totalAlerts - pendingAlerts - resolvedAlerts,
                        averageRiskScore = Math.Round(avgRiskScore * 100, 1)
                    },
                    generatedAt = DateTime.UtcNow
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting dashboard stats");
                return StatusCode(500, new { error = "Failed to retrieve dashboard statistics" });
            }
        }

        /// <summary>
        /// Get transaction trends over time
        /// </summary>
        [HttpGet("trends")]
        public async Task<IActionResult> GetTransactionTrends([FromQuery] int days = 30)
        {
            try
            {
                var startDate = DateTime.UtcNow.Date.AddDays(-days);

                var dailyStats = await _context.Transactions
                    .Where(t => t.Timestamp >= startDate)
                    .GroupBy(t => t.Timestamp.Date)
                    .Select(g => new
                    {
                        date = g.Key,
                        totalTransactions = g.Count(),
                        fraudTransactions = g.Count(t => t.IsFraud),
                        totalAmount = g.Sum(t => t.Amount),
                        fraudAmount = g.Where(t => t.IsFraud).Sum(t => t.Amount)
                    })
                    .OrderBy(x => x.date)
                    .ToListAsync();

                return Ok(new
                {
                    period = $"Last {days} days",
                    data = dailyStats
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting transaction trends");
                return StatusCode(500, new { error = "Failed to retrieve trends" });
            }
        }

        /// <summary>
        /// Get fraud analysis by country
        /// </summary>
        [HttpGet("fraud-by-country")]
        public async Task<IActionResult> GetFraudByCountry()
        {
            try
            {
                var countryStats = await _context.Transactions
                    .GroupBy(t => t.Country)
                    .Select(g => new
                    {
                        country = g.Key,
                        totalTransactions = g.Count(),
                        fraudTransactions = g.Count(t => t.IsFraud),
                        fraudRate = g.Count() > 0 
                            ? (double)g.Count(t => t.IsFraud) / g.Count() * 100 
                            : 0,
                        totalAmount = g.Sum(t => t.Amount),
                        fraudAmount = g.Where(t => t.IsFraud).Sum(t => t.Amount)
                    })
                    .OrderByDescending(x => x.fraudRate)
                    .ToListAsync();

                return Ok(countryStats);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting fraud by country");
                return StatusCode(500, new { error = "Failed to retrieve country statistics" });
            }
        }

        /// <summary>
        /// Get fraud analysis by device type
        /// </summary>
        [HttpGet("fraud-by-device")]
        public async Task<IActionResult> GetFraudByDevice()
        {
            try
            {
                var deviceStats = await _context.Transactions
                    .GroupBy(t => t.Device)
                    .Select(g => new
                    {
                        device = g.Key,
                        totalTransactions = g.Count(),
                        fraudTransactions = g.Count(t => t.IsFraud),
                        fraudRate = g.Count() > 0 
                            ? (double)g.Count(t => t.IsFraud) / g.Count() * 100 
                            : 0,
                        totalAmount = g.Sum(t => t.Amount)
                    })
                    .OrderByDescending(x => x.fraudRate)
                    .ToListAsync();

                return Ok(deviceStats);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting fraud by device");
                return StatusCode(500, new { error = "Failed to retrieve device statistics" });
            }
        }

        /// <summary>
        /// Get hourly fraud patterns
        /// </summary>
        [HttpGet("hourly-patterns")]
        public async Task<IActionResult> GetHourlyPatterns()
        {
            try
            {
                var hourlyStats = await _context.Transactions
                    .GroupBy(t => t.Timestamp.Hour)
                    .Select(g => new
                    {
                        hour = g.Key,
                        totalTransactions = g.Count(),
                        fraudTransactions = g.Count(t => t.IsFraud),
                        fraudRate = g.Count() > 0 
                            ? (double)g.Count(t => t.IsFraud) / g.Count() * 100 
                            : 0
                    })
                    .OrderBy(x => x.hour)
                    .ToListAsync();

                return Ok(hourlyStats);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting hourly patterns");
                return StatusCode(500, new { error = "Failed to retrieve hourly patterns" });
            }
        }

        /// <summary>
        /// Get all users list (Admin only)
        /// </summary>
        [HttpGet("users")]
        public async Task<IActionResult> GetAllUsers()
        {
            try
            {
                var users = await _context.Users
                    .Select(u => new
                    {
                        u.Id,
                        u.Email,
                        u.FirstName,
                        u.LastName,
                        u.Role,
                        accountCount = u.Accounts.Count,
                        totalBalance = u.Accounts.Sum(a => a.Balance),
                        transactionCount = u.Accounts.SelectMany(a => a.Transactions).Count(),
                        fraudTransactionCount = u.Accounts
                            .SelectMany(a => a.Transactions)
                            .Count(t => t.IsFraud)
                    })
                    .ToListAsync();

                return Ok(users);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting users");
                return StatusCode(500, new { error = "Failed to retrieve users" });
            }
        }

        /// <summary>
        /// Get high-risk transactions
        /// </summary>
        [HttpGet("high-risk-transactions")]
        public async Task<IActionResult> GetHighRiskTransactions([FromQuery] int limit = 50)
        {
            try
            {
                var highRiskTransactions = await _context.FraudAlerts
                    .Include(a => a.Transaction)
                        .ThenInclude(t => t.Account)
                            .ThenInclude(acc => acc.User)
                    .Where(a => a.RiskScore >= 0.7)
                    .OrderByDescending(a => a.RiskScore)
                    .Take(limit)
                    .Select(a => new
                    {
                        alertId = a.Id,
                        transactionId = a.TransactionId,
                        amount = a.Transaction.Amount,
                        country = a.Transaction.Country,
                        device = a.Transaction.Device,
                        type = a.Transaction.Type,
                        timestamp = a.Transaction.Timestamp,
                        riskScore = a.RiskScore,
                        status = a.Status,
                        userEmail = a.Transaction.Account.User.Email,
                        userName = $"{a.Transaction.Account.User.FirstName} {a.Transaction.Account.User.LastName}"
                    })
                    .ToListAsync();

                return Ok(highRiskTransactions);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting high-risk transactions");
                return StatusCode(500, new { error = "Failed to retrieve high-risk transactions" });
            }
        }

        /// <summary>
        /// Export data for Power BI
        /// </summary>
        [HttpGet("export/powerbi")]
        public async Task<IActionResult> ExportForPowerBI()
        {
            try
            {
                var transactions = await _context.Transactions
                    .Include(t => t.Account)
                        .ThenInclude(a => a.User)
                    .Select(t => new
                    {
                        transactionId = t.Id,
                        accountId = t.AccountId,
                        userId = t.Account.UserId,
                        userEmail = t.Account.User.Email,
                        amount = t.Amount,
                        type = t.Type,
                        country = t.Country,
                        device = t.Device,
                        isFraud = t.IsFraud,
                        timestamp = t.Timestamp,
                        hour = t.Timestamp.Hour,
                        dayOfWeek = t.Timestamp.DayOfWeek.ToString(),
                        month = t.Timestamp.Month,
                        year = t.Timestamp.Year
                    })
                    .ToListAsync();

                return Ok(new
                {
                    exportedAt = DateTime.UtcNow,
                    recordCount = transactions.Count,
                    data = transactions
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error exporting for Power BI");
                return StatusCode(500, new { error = "Failed to export data" });
            }
        }
    }
}
