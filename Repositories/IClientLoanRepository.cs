using HomeBankingAcc.Models;

namespace HomeBankingAcc.Repositories
{
    public interface IClientLoanRepository
    {
        void Save(ClientLoan clientLoan);
    }
}
