using HomeBankingAcc.Models;

namespace HomeBankingAcc.Repositories.Implementations
{
    public class LoanRepository : RepositoryBase<Loan>, ILoanRepository
    {
        public LoanRepository(HomeBankingContext repositoryContext) : base(repositoryContext)
        {
        }

        public Loan FindById(long id)
        {
            return FindByCondition(l => l.Id == id)
                .FirstOrDefault();
        }

        public IEnumerable<Loan> GetAllLoans()
        {
            return FindAll().ToList();
        }

        public void Save(Loan loan)
        {
            Create(loan);
            SaveChanges();
        }
    }
}
