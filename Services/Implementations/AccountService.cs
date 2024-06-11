using HomeBankingAcc.DTOs;
using HomeBankingAcc.Repositories;

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
    }
}
