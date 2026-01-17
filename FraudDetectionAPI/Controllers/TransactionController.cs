using FraudDetectionAPI.DTO.Transaction;
using FraudDetectionAPI.Models;
using FraudDetectionAPI.Services;
using FraudDetectionAPI.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace FraudDetectionAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class TransactionController : ControllerBase
    {
        private readonly ITransactionService _transactionService;
        private readonly ApplicationDbContext _context;

        public TransactionController(ITransactionService transactionService, ApplicationDbContext context)
        {
            _transactionService = transactionService;
            _context = context;
        }

        // --------------------------------------------
        // GET : /api/Transaction/my-transactions
        // Get current user's transactions with pagination
        // --------------------------------------------
        [HttpGet("my-transactions")]
        public async Task<ActionResult> GetMyTransactions(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20,
            [FromQuery] bool? isFraud = null,
            [FromQuery] string? type = null)
        {
            var userId = GetCurrentUserId();
            if (userId == 0) return Unauthorized();

            var accountIds = await _context.Accounts
                .Where(a => a.UserId == userId)
                .Select(a => a.Id)
                .ToListAsync();

            var baseQuery = _context.Transactions
                .Where(t => accountIds.Contains(t.AccountId));

            // Get total counts for all and suspicious BEFORE filtering
            var totalAllCount = await baseQuery.CountAsync();
            var totalSuspiciousCount = await baseQuery.CountAsync(t => t.IsFraud);

            var query = baseQuery.AsQueryable();

            if (isFraud.HasValue)
                query = query.Where(t => t.IsFraud == isFraud.Value);
            if (!string.IsNullOrEmpty(type))
                query = query.Where(t => t.Type == type);

            var filteredCount = await query.CountAsync();
            var transactions = await query
                .OrderByDescending(t => t.Timestamp)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Join(_context.Accounts, t => t.AccountId, a => a.Id, (t, a) => new TransactionResponseDTO
                {
                    Id = t.Id,
                    AccountId = t.AccountId,
                    AccountNumber = a.AccountNumber,
                    Amount = t.Amount,
                    Type = t.Type,
                    Country = t.Country,
                    Device = t.Device,
                    RecipientRIB = t.RecipientRIB,
                    Description = t.Description,
                    Timestamp = t.Timestamp,
                    IsFraud = t.IsFraud,
                    FraudReason = t.FraudReason
                })
                .ToListAsync();

            return Ok(new {
                data = transactions,
                pagination = new {
                    currentPage = page,
                    pageSize,
                    totalCount = filteredCount,
                    totalPages = (int)Math.Ceiling((double)filteredCount / pageSize)
                },
                counts = new {
                    all = totalAllCount,
                    suspicious = totalSuspiciousCount
                }
            });
        }

        // --------------------------------------------
        // GET : /api/Transaction/my-accounts
        // Get current user's accounts
        // --------------------------------------------
        [HttpGet("my-accounts")]
        public async Task<ActionResult> GetMyAccounts()
        {
            var userId = GetCurrentUserId();
            if (userId == 0) return Unauthorized();

            var accounts = await _context.Accounts
                .Where(a => a.UserId == userId)
                .Select(a => new {
                    a.Id,
                    a.AccountNumber,
                    a.Balance,
                    a.CreatedAt
                })
                .ToListAsync();

            return Ok(accounts);
        }

        // --------------------------------------------
        // GET : /api/Transaction/my-stats
        // Get current user's dashboard statistics
        // --------------------------------------------
        [HttpGet("my-stats")]
        public async Task<ActionResult> GetMyStats()
        {
            var userId = GetCurrentUserId();
            if (userId == 0) return Unauthorized();

            var accounts = await _context.Accounts.Where(a => a.UserId == userId).ToListAsync();
            var accountIds = accounts.Select(a => a.Id).ToList();
            var transactions = await _context.Transactions.Where(t => accountIds.Contains(t.AccountId)).ToListAsync();

            var stats = new {
                totalAccounts = accounts.Count,
                totalBalance = accounts.Sum(a => a.Balance),
                totalTransactions = transactions.Count,
                fraudTransactions = transactions.Count(t => t.IsFraud),
                fraudPercentage = transactions.Count > 0 
                    ? $"{((double)transactions.Count(t => t.IsFraud) / transactions.Count * 100):F1}%" 
                    : "0%",
                totalAmount = transactions.Sum(t => t.Amount),
                fraudAmount = transactions.Where(t => t.IsFraud).Sum(t => t.Amount),
                recentTransactions = transactions.OrderByDescending(t => t.Timestamp).Take(5).Select(t => new {
                    t.Id, t.Amount, t.Type, t.Country, t.Device, t.Timestamp, t.IsFraud
                })
            };

            return Ok(stats);
        }

        // --------------------------------------------
        // POST : /api/Transaction/create
        // Créer une nouvelle transaction
        // --------------------------------------------
        [HttpPost("create")]
        public async Task<ActionResult<TransactionResponseDTO>> CreateTransaction([FromBody] CreateTransactionDTO dto)
        {
            var userId = GetCurrentUserId();
            if (userId == 0) return Unauthorized();

            // Verify the account belongs to the user
            var account = await _context.Accounts.FirstOrDefaultAsync(a => a.Id == dto.AccountId && a.UserId == userId);
            if (account == null) return BadRequest(new { message = "Account not found or doesn't belong to you" });

            var transaction = new Transaction
            {
                AccountId = dto.AccountId,
                Amount = dto.Amount,
                Type = dto.Type,
                Country = dto.Country,
                Device = dto.Device,
                RecipientRIB = dto.RecipientRIB,
                Description = dto.Description
            };

            try
            {
                var created = await _transactionService.CreateTransactionAsync(transaction);

                var response = new TransactionResponseDTO
                {
                    Id = created.Id,
                    AccountId = created.AccountId,
                    AccountNumber = account.AccountNumber,
                    Amount = created.Amount,
                    Type = created.Type,
                    Country = created.Country,
                    Device = created.Device,
                    RecipientRIB = created.RecipientRIB,
                    Description = created.Description,
                    Timestamp = created.Timestamp,
                    IsFraud = created.IsFraud,
                    FraudReason = created.FraudReason
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        private int GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return int.TryParse(userIdClaim, out var userId) ? userId : 0;
        }

        // --------------------------------------------
        // GET : /api/Transaction/account/{accountId}
        // Récupérer toutes les transactions d’un compte
        // --------------------------------------------
        [HttpGet("account/{accountId}")]
        public async Task<ActionResult<IEnumerable<TransactionResponseDTO>>> GetByAccountId(int accountId)
        {
            var transactions = await _transactionService.GetByAccountIdAsync(accountId);

            var response = transactions.Select(t => new TransactionResponseDTO
            {
                Id = t.Id,
                AccountId = t.AccountId,
                Amount = t.Amount,
                Type = t.Type,
                Country = t.Country,
                Device = t.Device,
                RecipientRIB = t.RecipientRIB,
                Description = t.Description,
                Timestamp = t.Timestamp,
                IsFraud = t.IsFraud,
                FraudReason = t.FraudReason
            });

            return Ok(response);
        }

        // --------------------------------------------
        // GET : /api/Transaction/fraudulent
        // Récupérer toutes les transactions frauduleuses
        // (Admin seulement)
        // --------------------------------------------
        [HttpGet("fraudulent")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<IEnumerable<TransactionResponseDTO>>> GetFraudulent()
        {
            var transactions = await _transactionService.GetFraudulentAsync();

            var response = transactions.Select(t => new TransactionResponseDTO
            {
                Id = t.Id,
                AccountId = t.AccountId,
                Amount = t.Amount,
                Type = t.Type,
                Country = t.Country,
                Device = t.Device,
                RecipientRIB = t.RecipientRIB,
                Description = t.Description,
                Timestamp = t.Timestamp,
                IsFraud = t.IsFraud,
                FraudReason = t.FraudReason
            });

            return Ok(response);
        }

        // --------------------------------------------
        // GET : /api/Transaction/all
        // Récupérer toutes les transactions
        // (Admin)
        // --------------------------------------------
        [HttpGet("all")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<IEnumerable<TransactionResponseDTO>>> GetAll()
        {
            var transactions = await _transactionService.GetAllAsync();

            var response = transactions.Select(t => new TransactionResponseDTO
            {
                Id = t.Id,
                AccountId = t.AccountId,
                Amount = t.Amount,
                Type = t.Type,
                Country = t.Country,
                Device = t.Device,
                RecipientRIB = t.RecipientRIB,
                Description = t.Description,
                Timestamp = t.Timestamp,
                IsFraud = t.IsFraud,
                FraudReason = t.FraudReason
            });

            return Ok(response);
        }
    }
}
