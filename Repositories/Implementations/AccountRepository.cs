using HomeBankingAcc.Models;
using Microsoft.EntityFrameworkCore;

namespace HomeBankingAcc.Repositories.Implementations
{
    public class AccountRepository : RepositoryBase<Account>, IAccountRepository
    {
        public AccountRepository(HomeBankingContext repositoryContext) : base(repositoryContext)
        {
        }

        public Account FindById(long id)
        {
            return FindByCondition(a => a.Id == id)
                .Include(a => a.Transactions)
                .FirstOrDefault();
        }

        public IEnumerable<Account> GetAllAccounts()
        {
            return FindAll()
                .Include(c => c.Transactions)
                .ToList();
        }
        public void Save(Account account)
        {
            Create(account);
            SaveChanges();
        }
    }
}
