using HomeBankingAcc.DTOs;

namespace HomeBankingAcc.Services
{
    public interface IAccountService
    {
        AccountDTO GetAccountById(long id);
        IEnumerable<AccountDTO> GetAllAccounts();
        string createAccountNumber();
        AccountDTO createAccount(long clientID);
        IEnumerable<AccountDTO> getClientAccounts(long clientId);
    }
}
