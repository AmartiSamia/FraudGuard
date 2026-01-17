using FraudDetectionAPI.Models;
using FraudDetectionAPI.Repositories;
using System.Text;

namespace FraudDetectionAPI.Services
{
    public class TransactionService : ITransactionService
    {
        private readonly ITransactionRepository _transactionRepo;
        private readonly IAccountRepository _accountRepo;

        // Types de transactions sortantes (débit du compte)
        private static readonly HashSet<string> OutgoingTypes = new(StringComparer.OrdinalIgnoreCase)
        {
            "Virement", "Retrait", "Prelevement", "VirementInternational", 
            "Paiement", "PaiementEnLigne", "VirementInstantane", 
            "RemboursementCredit", "Facture"
        };

        // Types de transactions entrantes (crédit du compte)
        private static readonly HashSet<string> IncomingTypes = new(StringComparer.OrdinalIgnoreCase)
        {
            "Depot", "Salaire", "Emprunt"
        };

        public TransactionService(
            ITransactionRepository transactionRepo,
            IAccountRepository accountRepo)
        {
            _transactionRepo = transactionRepo;
            _accountRepo = accountRepo;
        }

        public async Task<Transaction> CreateTransactionAsync(Transaction transaction)
        {
            // Vérifier que le compte existe
            var account = await _accountRepo.GetByIdAsync(transaction.AccountId);
            if (account == null)
                throw new Exception("Compte introuvable.");

            // Timestamp auto si non fourni
            if (transaction.Timestamp == default)
                transaction.Timestamp = DateTime.UtcNow;

            // Déterminer si c'est une transaction sortante ou entrante
            bool isOutgoing = OutgoingTypes.Contains(transaction.Type);
            bool isIncoming = IncomingTypes.Contains(transaction.Type);

            if (!isOutgoing && !isIncoming)
            {
                throw new Exception($"Type de transaction invalide: {transaction.Type}. Types valides: Virement, Retrait, Depot, Prelevement, VirementInternational, Paiement, PaiementEnLigne, VirementInstantane, Emprunt, RemboursementCredit, Facture, Salaire");
            }

            // Détection de fraude
            bool isFraud = false;
            var reasons = new StringBuilder();

            if (transaction.Amount > 10000)
            {
                isFraud = true;
                reasons.Append("Montant supérieur à 10 000. ");
            }

            if (!string.Equals(transaction.Country, "MA", StringComparison.OrdinalIgnoreCase))
            {
                isFraud = true;
                reasons.Append("Pays différent de MA. ");
            }

            // Vérification supplémentaire pour les virements internationaux
            if (transaction.Type.Equals("VirementInternational", StringComparison.OrdinalIgnoreCase) && transaction.Amount > 5000)
            {
                isFraud = true;
                reasons.Append("Virement international supérieur à 5 000. ");
            }

            // Vérification pour les retraits ATM importants
            if (transaction.Type.Equals("Retrait", StringComparison.OrdinalIgnoreCase) && 
                transaction.Device?.Equals("ATM", StringComparison.OrdinalIgnoreCase) == true && 
                transaction.Amount > 2000)
            {
                isFraud = true;
                reasons.Append("Retrait ATM supérieur à 2 000. ");
            }

            transaction.IsFraud = isFraud;
            transaction.FraudReason = isFraud ? reasons.ToString().Trim() : null;

            // Mise à jour du solde
            if (isIncoming)
            {
                account.Balance += transaction.Amount;
            }
            else if (isOutgoing)
            {
                if (account.Balance < transaction.Amount)
                    throw new Exception($"Solde insuffisant. Solde actuel: {account.Balance:N2} MAD, Montant demandé: {transaction.Amount:N2} MAD");

                account.Balance -= transaction.Amount;
            }

            // Enregistrer la transaction
            await _transactionRepo.AddAsync(transaction);

            // Enregistrer toutes les modifications (transaction + solde)
            await _transactionRepo.SaveChangesAsync();

            return transaction;
        }

        public async Task<IEnumerable<Transaction>> GetByAccountIdAsync(int accountId)
        {
            return await _transactionRepo.GetByAccountIdAsync(accountId);
        }

        public async Task<IEnumerable<Transaction>> GetFraudulentAsync()
        {
            return await _transactionRepo.GetFraudulentAsync();
        }

        public async Task<IEnumerable<Transaction>> GetAllAsync()
        {
            return await _transactionRepo.GetAllAsync();
        }
    }
}
