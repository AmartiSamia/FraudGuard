using FraudDetectionAPI.Models;
using FraudDetectionAPI.Repositories;

namespace FraudDetectionAPI.Services
{
    public class AccountService : IAccountService
    {
        private readonly IAccountRepository _accountRepo; 


        public AccountService(IAccountRepository accountRepo)
        {
            _accountRepo = accountRepo;
        }


        public async Task<Account> CreateAccountAsync(int userId, decimal initialBalance)
        {
            var existing = await _accountRepo.GetByUserIdAsync(userId);
            if (existing != null)
                throw new Exception("Cet utilisateur a déjà un compte.");

            var account = new Account
            {
                UserId = userId,
                AccountNumber = $"ACC-{Guid.NewGuid().ToString().Substring(0, 8)}",
                Balance = initialBalance
            };

            await _accountRepo.AddAsync(account);
            await _accountRepo.SaveChangesAsync();

            return account;
        }

        public async Task<Account?> GetAccountByUserIdAsync(int userId)
        {
            return await _accountRepo.GetByUserIdAsync(userId);
        }
    }
}
