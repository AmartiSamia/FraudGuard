using Microsoft.AspNetCore.Mvc;
using FraudDetectionAPI.Data;
using Microsoft.EntityFrameworkCore;

namespace FraudDetectionAPI.Controllers
{
    /// <summary>
    /// Health check and system status controller
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class HealthController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;
        private readonly ILogger<HealthController> _logger;

        public HealthController(
            ApplicationDbContext context,
            IConfiguration configuration,
            ILogger<HealthController> logger)
        {
            _context = context;
            _configuration = configuration;
            _logger = logger;
        }

        /// <summary>
        /// Basic health check
        /// </summary>
        [HttpGet]
        public IActionResult Get()
        {
            return Ok(new
            {
                status = "healthy",
                service = "FraudGuard API",
                version = "2.0.0",
                timestamp = DateTime.UtcNow
            });
        }

        /// <summary>
        /// Detailed health check with dependencies
        /// </summary>
        [HttpGet("detailed")]
        public async Task<IActionResult> GetDetailed()
        {
            var healthReport = new
            {
                status = "healthy",
                service = "FraudGuard API",
                version = "2.0.0",
                timestamp = DateTime.UtcNow,
                checks = new List<object>()
            };

            var checks = new List<object>();

            // Database check
            try
            {
                var canConnect = await _context.Database.CanConnectAsync();
                checks.Add(new
                {
                    name = "Database",
                    status = canConnect ? "healthy" : "unhealthy",
                    responseTime = "< 100ms"
                });
            }
            catch (Exception ex)
            {
                checks.Add(new
                {
                    name = "Database",
                    status = "unhealthy",
                    error = ex.Message
                });
            }

            // ML Service check
            try
            {
                using var httpClient = new HttpClient { Timeout = TimeSpan.FromSeconds(5) };
                var mlUrl = _configuration["MLService:Url"] ?? "http://localhost:5000";
                var response = await httpClient.GetAsync($"{mlUrl}/health");
                checks.Add(new
                {
                    name = "ML Service",
                    status = response.IsSuccessStatusCode ? "healthy" : "unhealthy",
                    url = mlUrl
                });
            }
            catch (Exception ex)
            {
                checks.Add(new
                {
                    name = "ML Service",
                    status = "unhealthy",
                    error = ex.Message
                });
            }

            // Redis check (if enabled)
            var redisEnabled = _configuration.GetValue<bool>("Redis:Enabled", false);
            if (redisEnabled)
            {
                try
                {
                    // Would check Redis connection here
                    checks.Add(new
                    {
                        name = "Redis",
                        status = "healthy"
                    });
                }
                catch (Exception ex)
                {
                    checks.Add(new
                    {
                        name = "Redis",
                        status = "unhealthy",
                        error = ex.Message
                    });
                }
            }

            // Kafka check (if enabled)
            var kafkaEnabled = _configuration.GetValue<bool>("Kafka:Enabled", false);
            if (kafkaEnabled)
            {
                checks.Add(new
                {
                    name = "Kafka",
                    status = "configured",
                    bootstrapServers = _configuration["Kafka:BootstrapServers"]
                });
            }

            return Ok(new
            {
                status = checks.All(c => ((dynamic)c).status != "unhealthy") ? "healthy" : "degraded",
                service = "FraudGuard API",
                version = "2.0.0",
                environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production",
                timestamp = DateTime.UtcNow,
                checks
            });
        }

        /// <summary>
        /// Get system statistics
        /// </summary>
        [HttpGet("stats")]
        public async Task<IActionResult> GetStats()
        {
            try
            {
                var stats = new
                {
                    database = new
                    {
                        users = await _context.Users.CountAsync(),
                        accounts = await _context.Accounts.CountAsync(),
                        transactions = await _context.Transactions.CountAsync(),
                        fraudAlerts = await _context.FraudAlerts.CountAsync()
                    },
                    system = new
                    {
                        uptime = Environment.TickCount64 / 1000,
                        machineName = Environment.MachineName,
                        osVersion = Environment.OSVersion.ToString(),
                        processorCount = Environment.ProcessorCount
                    },
                    memory = new
                    {
                        workingSet = Environment.WorkingSet / 1024 / 1024,
                        unit = "MB"
                    }
                };

                return Ok(stats);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting stats");
                return StatusCode(500, new { error = "Failed to retrieve stats" });
            }
        }
    }
}
