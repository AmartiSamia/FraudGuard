using FraudDetectionAPI.Data;
using FraudDetectionAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FraudDetectionAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class DashboardController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ITransactionService _transactionService;
        private readonly IUserService _userService;
        private readonly IFraudAlertService _fraudAlertService;

        public DashboardController(
            ApplicationDbContext context,
            ITransactionService transactionService,
            IUserService userService,
            IFraudAlertService fraudAlertService)
        {
            _context = context;
            _transactionService = transactionService;
            _userService = userService;
            _fraudAlertService = fraudAlertService;
        }

        // ============================================
        // Admin Only - Dashboard Statistics
        // ============================================

        /// <summary>
        /// Get overall fraud detection statistics (Admin only)
        /// </summary>
        [HttpGet("statistics")]
        [Authorize(Policy = "AdminOnly")]
        public async Task<ActionResult<object>> GetStatistics()
        {
            try
            {
                var totalTransactions = await _context.Transactions.CountAsync();
                var fraudTransactions = await _context.Transactions.CountAsync(t => t.IsFraud);
                var totalAmount = await _context.Transactions.SumAsync(t => t.Amount);
                var fraudAmount = await _context.Transactions
                    .Where(t => t.IsFraud)
                    .SumAsync(t => t.Amount);
                var totalUsers = await _context.Users.CountAsync();
                var totalAccounts = await _context.Accounts.CountAsync();
                var pendingAlerts = await _context.FraudAlerts.CountAsync(f => f.Status == "Pending");

                var statistics = new
                {
                    totalTransactions,
                    fraudTransactions,
                    fraudPercentage = totalTransactions > 0 ? ((double)fraudTransactions / totalTransactions * 100).ToString("F2") + "%" : "0%",
                    totalAmount = totalAmount.ToString("F2"),
                    fraudAmount = fraudAmount.ToString("F2"),
                    totalUsers,
                    totalAccounts,
                    pendingAlerts,
                    timestamp = DateTime.UtcNow
                };

                return Ok(statistics);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Error retrieving statistics", error = ex.Message });
            }
        }

        /// <summary>
        /// Get fraud statistics by time period (Admin only)
        /// </summary>
        [HttpGet("fraud-by-period")]
        [Authorize(Policy = "AdminOnly")]
        public async Task<ActionResult<object>> GetFraudByPeriod([FromQuery] int days = 30)
        {
            try
            {
                var startDate = DateTime.UtcNow.AddDays(-days);

                var dailyStats = await _context.Transactions
                    .Where(t => t.Timestamp >= startDate)
                    .GroupBy(t => t.Timestamp.Date)
                    .Select(g => new
                    {
                        date = g.Key,
                        totalCount = g.Count(),
                        fraudCount = g.Count(t => t.IsFraud),
                        totalAmount = g.Sum(t => t.Amount),
                        fraudAmount = g.Where(t => t.IsFraud).Sum(t => t.Amount)
                    })
                    .OrderBy(x => x.date)
                    .ToListAsync();

                return Ok(dailyStats);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Error retrieving fraud statistics", error = ex.Message });
            }
        }

        /// <summary>
        /// Get fraud statistics by country (Admin only)
        /// </summary>
        [HttpGet("fraud-by-country")]
        [Authorize(Policy = "AdminOnly")]
        public async Task<ActionResult<object>> GetFraudByCountry()
        {
            try
            {
                var countryStats = await _context.Transactions
                    .GroupBy(t => t.Country)
                    .Select(g => new
                    {
                        country = g.Key,
                        totalCount = g.Count(),
                        fraudCount = g.Count(t => t.IsFraud),
                        fraudPercentage = g.Count() > 0 ? ((double)g.Count(t => t.IsFraud) / g.Count() * 100).ToString("F2") : "0"
                    })
                    .OrderByDescending(x => x.fraudCount)
                    .ToListAsync();

                return Ok(countryStats);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Error retrieving country statistics", error = ex.Message });
            }
        }

        /// <summary>
        /// Get fraud statistics by device type (Admin only)
        /// </summary>
        [HttpGet("fraud-by-device")]
        [Authorize(Policy = "AdminOnly")]
        public async Task<ActionResult<object>> GetFraudByDevice()
        {
            try
            {
                var deviceStats = await _context.Transactions
                    .GroupBy(t => t.Device)
                    .Select(g => new
                    {
                        device = g.Key,
                        totalCount = g.Count(),
                        fraudCount = g.Count(t => t.IsFraud),
                        fraudPercentage = g.Count() > 0 ? ((double)g.Count(t => t.IsFraud) / g.Count() * 100).ToString("F2") : "0"
                    })
                    .OrderByDescending(x => x.fraudCount)
                    .ToListAsync();

                return Ok(deviceStats);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Error retrieving device statistics", error = ex.Message });
            }
        }

        // ============================================
        // User Dashboard - Personal Statistics
        // ============================================

        /// <summary>
        /// Get user's personal transaction statistics (User/Admin)
        /// </summary>
        [HttpGet("user-statistics/{accountId}")]
        [Authorize(Policy = "AdminOrUser")]
        public async Task<ActionResult<object>> GetUserStatistics(int accountId)
        {
            try
            {
                // Check authorization - user can only see their own data
                var account = await _context.Accounts.FindAsync(accountId);
                if (account == null)
                    return NotFound(new { message = "Account not found" });

                var transactions = await _context.Transactions
                    .Where(t => t.AccountId == accountId)
                    .ToListAsync();

                var userStats = new
                {
                    accountId,
                    totalTransactions = transactions.Count,
                    fraudTransactions = transactions.Count(t => t.IsFraud),
                    fraudPercentage = transactions.Count > 0 
                        ? ((double)transactions.Count(t => t.IsFraud) / transactions.Count * 100).ToString("F2") + "%"
                        : "0%",
                    totalAmount = transactions.Sum(t => t.Amount).ToString("F2"),
                    fraudAmount = transactions.Where(t => t.IsFraud).Sum(t => t.Amount).ToString("F2")
                };

                return Ok(userStats);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Error retrieving user statistics", error = ex.Message });
            }
        }

        /// <summary>
        /// Get recent suspicious transactions (Admin only)
        /// </summary>
        [HttpGet("recent-suspicious")]
        [Authorize(Policy = "AdminOnly")]
        public async Task<ActionResult<object>> GetRecentSuspiciousTransactions([FromQuery] int limit = 20)
        {
            try
            {
                var suspiciousTransactions = await _context.Transactions
                    .Where(t => t.IsFraud)
                    .Include(t => t.Account)
                    .ThenInclude(a => a.User)
                    .OrderByDescending(t => t.Timestamp)
                    .Take(limit)
                    .Select(t => new
                    {
                        t.Id,
                        t.AccountId,
                        accountOwner = t.Account.User.Email,
                        t.Amount,
                        t.Type,
                        t.Country,
                        t.Device,
                        t.Timestamp,
                        t.FraudReason
                    })
                    .ToListAsync();

                return Ok(suspiciousTransactions);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Error retrieving suspicious transactions", error = ex.Message });
            }
        }

        /// <summary>
        /// Get pending fraud alerts (Admin only)
        /// </summary>
        [HttpGet("pending-alerts")]
        [Authorize(Policy = "AdminOnly")]
        public async Task<ActionResult<object>> GetPendingAlerts([FromQuery] int limit = 50)
        {
            try
            {
                var pendingAlerts = await _context.FraudAlerts
                    .Where(f => f.Status == "Pending")
                    .Include(f => f.Transaction)
                    .OrderByDescending(f => f.CreatedAt)
                    .Take(limit)
                    .Select(f => new
                    {
                        f.Id,
                        f.TransactionId,
                        f.RiskScore,
                        f.Status,
                        f.CreatedAt,
                        transactionAmount = f.Transaction.Amount,
                        transactionCountry = f.Transaction.Country
                    })
                    .ToListAsync();

                return Ok(pendingAlerts);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Error retrieving pending alerts", error = ex.Message });
            }
        }

        /// <summary>
        /// Get high-risk accounts (Admin only)
        /// </summary>
        [HttpGet("high-risk-accounts")]
        [Authorize(Policy = "AdminOnly")]
        public async Task<ActionResult<object>> GetHighRiskAccounts([FromQuery] int limit = 20)
        {
            try
            {
                var highRiskAccounts = await _context.Accounts
                    .Include(a => a.User)
                    .Select(a => new
                    {
                        a.Id,
                        accountOwner = a.User.Email,
                        a.AccountNumber,
                        totalTransactions = a.Transactions.Count,
                        fraudCount = a.Transactions.Count(t => t.IsFraud),
                        riskScore = a.Transactions.Count > 0 
                            ? (double)a.Transactions.Count(t => t.IsFraud) / a.Transactions.Count * 100
                            : 0
                    })
                    .Where(a => a.fraudCount > 0)
                    .OrderByDescending(a => a.riskScore)
                    .Take(limit)
                    .ToListAsync();

                return Ok(highRiskAccounts);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Error retrieving high-risk accounts", error = ex.Message });
            }
        }
    }
}
