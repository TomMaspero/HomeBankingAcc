using HomeBankingAcc.Models;

namespace HomeBankingAcc.Repositories
{
    public interface IAccountRepository
    {
        IEnumerable<Account> GetAllAccounts();
        IEnumerable<Account> GetAccountsByClient(long clientId);
        Account FindById(long id);
        Account GetAccountByNumber(string number);
        void Save(Account account); 
    }
}
