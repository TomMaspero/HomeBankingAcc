using HomeBankingAcc.DTOs;

namespace HomeBankingAcc.Services
{
    public interface IAccountService
    {
        AccountDTO GetAccountById(long id);
        IEnumerable<AccountDTO> GetAllAccounts();
    }
}
