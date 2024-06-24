using HomeBankingAcc.DTOs;
using HomeBankingAcc.Models;
using HomeBankingAcc.Repositories;
using Microsoft.IdentityModel.Tokens;

namespace HomeBankingAcc.Services.Implementations
{
    public class LoanService : ILoanService
    {
        private readonly ILoanRepository _loanRepository;
        private readonly IAccountRepository _accountRepository;
        private readonly IClientLoanRepository _clientLoanRepository;
        private readonly ITransactionRepository _transactionRepository;
        public LoanService(ILoanRepository loanRepository, IAccountRepository accountRepository, IClientLoanRepository clientLoanRepository, ITransactionRepository transactionRepository)
        {
            _loanRepository = loanRepository;
            _accountRepository = accountRepository;
            _clientLoanRepository = clientLoanRepository;
            _transactionRepository = transactionRepository;
        }
        public IEnumerable<LoanDTO> GetAllLoans()
        {
            var loans = _loanRepository.GetAllLoans();
            var loansDTO = loans.Select(l => new LoanDTO(l)).ToList();
            return loansDTO;
        }

        public LoanDTO GetLoanById(long id)
        {
            var loan = _loanRepository.FindById(id);
            var loanDTO = new LoanDTO(loan);
            return loanDTO;
        }
        public (bool isValid, string errorMessage) validateLoan(long clientId, LoanApplicationDTO loanApplicationDTO)
        {
            Loan loan = _loanRepository.FindById(loanApplicationDTO.LoanId);
            Account account = _accountRepository.GetAccountByNumber(loanApplicationDTO.ToAccountNumber);
            var clientAccounts = _accountRepository.GetAccountsByClient(clientId);
            if (loanApplicationDTO.Amount <= 0)
                return (false, "Error: invalid loan amount");
            if (loan == null)
                return (false, "Error: loan does not exist");
            if (loanApplicationDTO.Amount > loan.MaxAmount)
                return (false, "Error: loan cannot exceed the maximum amount");
            if (loanApplicationDTO.Payments.IsNullOrEmpty())
                return (false, "Error: Payments cannot be empty");
            if (account == null)
                return (false, "Error: the account does not exist");
            if (!clientAccounts.Any(acc => acc.Number.Equals(account.Number)))
                return (false, "Error: the account does not belong to the current client");
            return (true, string.Empty);
        }
        public NewClientLoanDTO createLoan(long clientId, LoanApplicationDTO loanApplicationDTO)
        {
            Loan loan = _loanRepository.FindById(loanApplicationDTO.LoanId);
            Account account = _accountRepository.GetAccountByNumber(loanApplicationDTO.ToAccountNumber);

            ClientLoan newClientLoan = new ClientLoan
            {
                Amount = loanApplicationDTO.Amount * 1.2,
                Payments = loanApplicationDTO.Payments,
                ClientId = clientId,
                LoanId = loanApplicationDTO.LoanId,
            };
            _clientLoanRepository.Save(newClientLoan);

            Transaction loanTransaction = new Transaction
            {
                Type = TransactionType.CREDIT,
                Amount = loanApplicationDTO.Amount,
                Description = "Loan: " + loan.Name + " to Account " + account.Number,
                Date = DateTime.Now,
                AccountId = account.Id,
            };
            _transactionRepository.Save(loanTransaction);

            account.Balance += loanApplicationDTO.Amount;
            _accountRepository.Save(account);

            NewClientLoanDTO newClientLoanDTO = new NewClientLoanDTO(newClientLoan, loan, loanTransaction);

            return newClientLoanDTO;
        }
    }
}
