using FraudDetectionAPI.Models;
using FraudDetectionAPI.Repositories;

namespace FraudDetectionAPI.Services
{
    public class FraudAlertService : IFraudAlertService
    {
        private readonly IFraudAlertRepository _fraudAlertRepo;

        public FraudAlertService(IFraudAlertRepository fraudAlertRepo)
        {
            _fraudAlertRepo = fraudAlertRepo;
        }

        public async Task<IEnumerable<FraudAlert>> GetAllAsync()
        {
            return await _fraudAlertRepo.GetAllAsync();
        }

        public async Task<IEnumerable<FraudAlert>> GetByStatusAsync(string status)
        {
            return await _fraudAlertRepo.GetByStatusAsync(status);
        }

        public async Task<FraudAlert?> GetByIdAsync(int id)
        {
            return await _fraudAlertRepo.GetByIdAsync(id);
        }

        public async Task<FraudAlert> UpdateStatusAsync(int id, string newStatus)
        {
            var alert = await _fraudAlertRepo.GetByIdAsync(id);
            if (alert == null)
                throw new Exception("Alerte introuvable.");

            alert.Status = newStatus;

            await _fraudAlertRepo.SaveChangesAsync();

            return alert;
        }
    }
}
