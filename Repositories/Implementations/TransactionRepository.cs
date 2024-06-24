using HomeBankingAcc.Models;

namespace HomeBankingAcc.Repositories.Implementations
{
    public class TransactionRepository : RepositoryBase<Transaction>, ITransactionRepository
    {
        public TransactionRepository(HomeBankingContext repositoryContext) : base(repositoryContext)
        {
        }

        public Transaction FindById(long id)
        {
            return FindByCondition(t => t.Id == id)
                .FirstOrDefault();
        }

        public IEnumerable<Transaction> GetAllTransactions()
        {
            return FindAll().ToList();
        }

        public void Save(Transaction transaction)
        {
            Create(transaction);
            SaveChanges();
        }
    }
}
