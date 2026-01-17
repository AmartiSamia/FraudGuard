using FraudDetectionAPI.Models;
using FraudDetectionAPI.Repositories;
using BCrypt.Net;

namespace FraudDetectionAPI.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _repo;

        public UserService(IUserRepository repo)
        {
            _repo = repo;
        }

        public async Task<User> RegisterAsync(User user)
        {
            user.Password = BCrypt.Net.BCrypt.HashPassword(user.Password);
            await _repo.AddUserAsync(user);
            await _repo.SaveChangesAsync();
            return user;
        }

        public async Task<User?> AuthenticateAsync(string email, string password)
        {
            var user = await _repo.GetByEmailAsync(email);
            if (user == null) return null;

            bool valid = BCrypt.Net.BCrypt.Verify(password, user.Password);
            return valid ? user : null;
        }

        public async Task<IEnumerable<User>> GetAllUsersAsync()
        {
            return await _repo.GetAllUsersAsync();
        }
    }
}
