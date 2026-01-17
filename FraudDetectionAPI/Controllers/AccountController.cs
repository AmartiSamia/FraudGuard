using FraudDetectionAPI.DTO.Account;
using FraudDetectionAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using FraudDetectionAPI.Models;

namespace FraudDetectionAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AccountController : ControllerBase
    {
        private readonly IAccountService _service;

        public AccountController(IAccountService service)
        {
            _service = service;
        }

        // Créer un compte pour un user
        [Authorize]
        [HttpPost("create")]
        public async Task<IActionResult> Create(AccountCreateDTO dto)
        {
            var account = await _service.CreateAccountAsync(dto.UserId, dto.InitialBalance);

            var response = new AccountResponseDTO
            {
                Id = account.Id,
                UserId = account.UserId,
                AccountNumber = account.AccountNumber,
                Balance = account.Balance,
                CreatedAt = account.CreatedAt
            };

            return Ok(response);
        }

        // Récupérer le compte d'un user
        [Authorize]
        [HttpGet("user/{userId}")]
        public async Task<IActionResult> Get(int userId)
        {
            var account = await _service.GetAccountByUserIdAsync(userId);
            if (account == null) return NotFound();

            var response = new AccountResponseDTO
            {
                Id = account.Id,
                UserId = account.UserId,
                AccountNumber = account.AccountNumber,
                Balance = account.Balance,
                CreatedAt = account.CreatedAt
            };

            return Ok(response);
        }
    }
}
