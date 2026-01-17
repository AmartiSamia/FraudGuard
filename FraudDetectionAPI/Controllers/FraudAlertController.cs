using FraudDetectionAPI.DTO.FraudAlert;
using FraudDetectionAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FraudDetectionAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin")] // seules les admins gèrent les alertes
    public class FraudAlertController : ControllerBase
    {
        private readonly IFraudAlertService _service;

        public FraudAlertController(IFraudAlertService service)
        {
            _service = service;
        }

        // GET: /api/FraudAlert/all
        [HttpGet("all")]
        public async Task<ActionResult<IEnumerable<FraudAlertResponseDTO>>> GetAll()
        {
            var alerts = await _service.GetAllAsync();

            var response = alerts.Select(a => new FraudAlertResponseDTO
            {
                Id = a.Id,
                TransactionId = a.TransactionId,
                RiskScore = a.RiskScore,
                Reason = a.Reason,
                Status = a.Status,
                CreatedAt = a.CreatedAt
            });

            return Ok(response);
        }

        // GET: /api/FraudAlert/status/{status}
        // ex: /api/FraudAlert/status/New
        [HttpGet("status/{status}")]
        public async Task<ActionResult<IEnumerable<FraudAlertResponseDTO>>> GetByStatus(string status)
        {
            var alerts = await _service.GetByStatusAsync(status);

            var response = alerts.Select(a => new FraudAlertResponseDTO
            {
                Id = a.Id,
                TransactionId = a.TransactionId,
                RiskScore = a.RiskScore,
                Reason = a.Reason,
                Status = a.Status,
                CreatedAt = a.CreatedAt
            });

            return Ok(response);
        }

        // PUT: /api/FraudAlert/{id}/status
        [HttpPut("{id}/status")]
        public async Task<ActionResult<FraudAlertResponseDTO>> UpdateStatus(int id, [FromBody] UpdateFraudAlertStatusDTO dto)
        {
            try
            {
                var updated = await _service.UpdateStatusAsync(id, dto.Status);

                var response = new FraudAlertResponseDTO
                {
                    Id = updated.Id,
                    TransactionId = updated.TransactionId,
                    RiskScore = updated.RiskScore,
                    Reason = updated.Reason,
                    Status = updated.Status,
                    CreatedAt = updated.CreatedAt
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}