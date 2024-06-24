using HomeBankingAcc.DTOs;
using HomeBankingAcc.Models;
using HomeBankingAcc.Repositories;
using System.Security.Cryptography;

namespace HomeBankingAcc.Services.Implementations
{
    public class AccountService : IAccountService
    {
        private readonly IAccountRepository _accountRepository;
        public AccountService(IAccountRepository accountRepository)
        {
            _accountRepository = accountRepository;
        }
        public AccountDTO GetAccountById(long id)
        {
            var account = _accountRepository.FindById(id);
            var accountDTO = new AccountDTO(account);
            return accountDTO;
        }
        public IEnumerable<AccountDTO> GetAllAccounts()
        {
            var accounts = _accountRepository.GetAllAccounts();
            var accountsDTO = accounts.Select(a => new AccountDTO(a)).ToList();
            return accountsDTO;
        }
        public string createAccountNumber()
        {
            string accNumber = "";
            do
            {
                accNumber = "VIN-" + RandomNumberGenerator.GetInt32(0, 99999999);
            } while (_accountRepository.GetAccountByNumber(accNumber) != null);
            return accNumber;
        }
        public AccountDTO createAccount(long clientId)
        {
            Account newAccount = new Account
            {
                Number = createAccountNumber(),
                CreationDate = DateTime.Now,
                Balance = 0,
                ClientId = clientId
            };
            _accountRepository.Save(newAccount);
            return new AccountDTO(newAccount);
        }
        public IEnumerable<AccountDTO> getClientAccounts(long clientId)
        {
            var clientAccounts = _accountRepository.GetAccountsByClient(clientId);
            var clientAccountsDTO = clientAccounts.Select(ca => new AccountDTO(ca)).ToList();
            return clientAccountsDTO;
        }

    }
}
