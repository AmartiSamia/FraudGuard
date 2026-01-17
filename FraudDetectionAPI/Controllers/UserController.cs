using FraudDetectionAPI.Data;
using FraudDetectionAPI.DTO.User;
using FraudDetectionAPI.Models;
using FraudDetectionAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace FraudDetectionAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _service;
        private readonly IConfiguration _config;
        private readonly ApplicationDbContext _context;

        public UserController(IUserService service, IConfiguration config, ApplicationDbContext context)
        {
            _service = service;
            _config = config;
            _context = context;
        }

        private int GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return int.Parse(userIdClaim ?? "0");
        }

        // =============================
        // POST: api/User/register
        // =============================
        [HttpPost("register")]
        public async Task<ActionResult<UserResponseDTO>> Register([FromBody] UserRegisterDTO dto)
        {
            // DTO -> Model
            var user = new User
            {
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                Email = dto.Email,
                Password = dto.Password,
                Role = dto.Role
            };

            var created = await _service.RegisterAsync(user);

            // Model -> DTO
            var response = new UserResponseDTO
            {
                Id = created.Id,
                FirstName = created.FirstName,
                LastName = created.LastName,
                Email = created.Email,
                Role = created.Role
            };

            return Ok(response);
        }

        // =============================
        // POST: api/User/login
        // =============================
        [HttpPost("login")]
        public async Task<ActionResult<string>> Login([FromBody] UserLoginDTO login)
        {
            var user = await _service.AuthenticateAsync(login.Email, login.Password);
            if (user == null) return Unauthorized("Email ou mot de passe incorrect");

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_config["Jwt:Key"]);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new Claim(ClaimTypes.Role, user.Role)
                }),
                Expires = DateTime.UtcNow.AddMinutes(double.Parse(_config["Jwt:DurationInMinutes"])),
                Issuer = _config["Jwt:Issuer"],
                Audience = _config["Jwt:Audience"],
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature
                )
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return Ok(tokenHandler.WriteToken(token));
        }

        // =============================
        // GET: api/User/all   (Admin only)
        // =============================
        [Authorize(Roles = "Admin")]
        [HttpGet("all")]
        public async Task<ActionResult<IEnumerable<UserResponseDTO>>> GetAll()
        {
            var users = await _service.GetAllUsersAsync();

            var result = users.Select(u => new UserResponseDTO
            {
                Id = u.Id,
                FirstName = u.FirstName,
                LastName = u.LastName,
                Email = u.Email,
                Role = u.Role
            });

            return Ok(result);
        }

        // =============================
        // GET: api/User/profile - Get current user profile
        // =============================
        [Authorize]
        [HttpGet("profile")]
        public async Task<ActionResult> GetProfile()
        {
            var userId = GetCurrentUserId();
            var user = await _context.Users
                .Include(u => u.Accounts)
                    .ThenInclude(a => a.Transactions)
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null) return NotFound(new { message = "User not found" });

            var totalTransactions = user.Accounts.SelectMany(a => a.Transactions).Count();
            var fraudTransactions = user.Accounts.SelectMany(a => a.Transactions).Count(t => t.IsFraud);

            return Ok(new {
                id = user.Id,
                firstName = user.FirstName,
                lastName = user.LastName,
                email = user.Email,
                role = user.Role,
                createdAt = user.CreatedAt,
                stats = new {
                    accountCount = user.Accounts.Count,
                    totalBalance = user.Accounts.Sum(a => a.Balance),
                    totalTransactions,
                    fraudTransactions,
                    safeTransactions = totalTransactions - fraudTransactions
                },
                accounts = user.Accounts.Select(a => new {
                    a.Id,
                    a.AccountNumber,
                    a.Balance,
                    a.CreatedAt,
                    transactionCount = a.Transactions.Count
                })
            });
        }

        // =============================
        // PUT: api/User/profile - Update current user profile
        // =============================
        [Authorize]
        [HttpPut("profile")]
        public async Task<ActionResult> UpdateProfile([FromBody] UpdateProfileRequest request)
        {
            var userId = GetCurrentUserId();
            var user = await _context.Users.FindAsync(userId);

            if (user == null) return NotFound(new { message = "User not found" });

            if (!string.IsNullOrEmpty(request.FirstName)) user.FirstName = request.FirstName;
            if (!string.IsNullOrEmpty(request.LastName)) user.LastName = request.LastName;
            if (!string.IsNullOrEmpty(request.Email))
            {
                var existing = await _context.Users.FirstOrDefaultAsync(u => u.Email == request.Email && u.Id != userId);
                if (existing != null) return BadRequest(new { message = "Email already in use" });
                user.Email = request.Email;
            }

            await _context.SaveChangesAsync();
            return Ok(new { message = "Profile updated successfully" });
        }

        // =============================
        // PUT: api/User/change-password
        // =============================
        [Authorize]
        [HttpPut("change-password")]
        public async Task<ActionResult> ChangePassword([FromBody] ChangePasswordRequest request)
        {
            var userId = GetCurrentUserId();
            var user = await _context.Users.FindAsync(userId);

            if (user == null) return NotFound(new { message = "User not found" });

            // Verify current password
            if (!BCrypt.Net.BCrypt.Verify(request.CurrentPassword, user.Password))
                return BadRequest(new { message = "Current password is incorrect" });

            // Update password
            user.Password = BCrypt.Net.BCrypt.HashPassword(request.NewPassword);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Password changed successfully" });
        }
    }

    public class UpdateProfileRequest
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Email { get; set; }
    }

    public class ChangePasswordRequest
    {
        public string CurrentPassword { get; set; } = "";
        public string NewPassword { get; set; } = "";
    }
}
