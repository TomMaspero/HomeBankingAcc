using HomeBankingAcc.DTOs;

namespace HomeBankingAcc.Services
{
    public interface ILoanService
    {
        LoanDTO GetLoanById(long id);
        IEnumerable<LoanDTO> GetAllLoans();
        public (bool isValid, string errorMessage) validateLoan(long clientId, LoanApplicationDTO loanApplicationDTO);
        public NewClientLoanDTO createLoan(long clientId, LoanApplicationDTO loanApplicationDTO);
    }
}
