using HomeBankingAcc.DTOs;
using HomeBankingAcc.Models;
using HomeBankingAcc.Repositories;
using Microsoft.IdentityModel.Tokens;

namespace HomeBankingAcc.Services.Implementations
{
    public class TransactionService : ITransactionService
    {
        private readonly ITransactionRepository _transactionRepository;
        private readonly IAccountRepository _accountRepository;
        public TransactionService(ITransactionRepository transactionRepository, IAccountRepository accountRepository)
        {
            _transactionRepository = transactionRepository;
            _accountRepository = accountRepository;
        }
        public TransactionDTO GetTransactionById(long id)
        {
            var transaction = _transactionRepository.FindById(id);
            var transactionDTO = new TransactionDTO(transaction);
            return transactionDTO;
        }
        public IEnumerable<TransactionDTO> GetAllTransactions()
        {
            var transactions = _transactionRepository.GetAllTransactions();
            var transactionsDTO = transactions.Select(t => new TransactionDTO(t)).ToList();
            return transactionsDTO;
        }
        public (bool isValid, string errorMessage) validateTransaction(long clientId, TransferDTO transferDTO)
        {
            if (transferDTO.Amount <= 0 || transferDTO.FromAccountNumber.IsNullOrEmpty()
                    || transferDTO.ToAccountNumber.IsNullOrEmpty() || transferDTO.Description.IsNullOrEmpty())
                return (false, "Error: Invalid Transaction");
            if (transferDTO.FromAccountNumber.Equals(transferDTO.ToAccountNumber))
                return (false, "Error: The account of origin cannot be the same as the destination account");
            Account accountFrom = _accountRepository.GetAccountByNumber(transferDTO.FromAccountNumber);
            Account accountTo = _accountRepository.GetAccountByNumber(transferDTO.ToAccountNumber);
            if (accountFrom == null)
                return (false, "Error: the account of origin does not exist");
            if (accountFrom.ClientId != clientId)
                return (false, "Error: the account of origin does not belong to the current client");
            if (accountTo == null)
                return (false, "Error: the destination account does not exist");
            if (accountFrom.Balance - transferDTO.Amount < 0)
                return (false, "Error: Insufficient funds");
            return (true, string.Empty);
        }
        public TransactionDTO[] createTransaction(TransferDTO transferDTO)
        {
            Account accountFrom = _accountRepository.GetAccountByNumber(transferDTO.FromAccountNumber);
            Account accountTo = _accountRepository.GetAccountByNumber(transferDTO.ToAccountNumber);
            Transaction[] transactions =
            {
                new Transaction
                {
                    Type = TransactionType.DEBIT,
                    Amount = -transferDTO.Amount,
                    Description = transferDTO.Description + " - Transfer sent to " + accountTo.Number.ToString(),
                    Date = DateTime.Now,
                    AccountId = accountFrom.Id,
                },
                new Transaction
                {
                    Type = TransactionType.CREDIT,
                    Amount = transferDTO.Amount,
                    Description = transferDTO.Description + " - Transfer received from " + accountFrom.Number.ToString(),
                    Date = DateTime.Now,
                    AccountId = accountTo.Id,
                }
            };
            TransactionDTO[] transactionDTOs = new TransactionDTO[2];
            int i = 0;
            foreach (var transaction in transactions)
            {
                _transactionRepository.Save(transaction);
                TransactionDTO transactionDTO = new TransactionDTO(transaction);
                transactionDTOs[i] = transactionDTO;
                i++;
            }

            accountFrom.Balance -= transferDTO.Amount;
            _accountRepository.Save(accountFrom);
            accountTo.Balance += transferDTO.Amount;
            _accountRepository.Save(accountTo);

            return transactionDTOs;
        }
    }
}
