using FraudDetectionAPI.Models;

namespace FraudDetectionAPI.Services
{
    public interface IUserService
    {
        Task<User?> AuthenticateAsync(string email, string password);
        Task<User> RegisterAsync(User user);
        Task<IEnumerable<User>> GetAllUsersAsync();
    }
}
