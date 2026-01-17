using FraudDetectionAPI.Models;

namespace FraudDetectionAPI.Repositories
{
    public interface IUserRepository
    {
        Task<User?> GetByEmailAsync(string email);
        Task AddUserAsync(User user);
        Task<IEnumerable<User>> GetAllUsersAsync();
        Task SaveChangesAsync();

    }
}
