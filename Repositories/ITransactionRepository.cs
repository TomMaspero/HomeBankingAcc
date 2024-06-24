using HomeBankingAcc.Models;

namespace HomeBankingAcc.Repositories
{
    public interface ITransactionRepository
    {
        IEnumerable<Transaction> GetAllTransactions();
        void Save(Transaction transaction);
        Transaction FindById(long id);
    }
}
