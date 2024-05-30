using HomeBankingAcc.Models;
using Microsoft.Identity.Client;

namespace HomeBankingAcc.DTOs
{
    public class ClientLoanDTO
    {
        public long Id { get; set; }
        public long LoanId { get; set; }
        public string Name { get; set; }
        public double Amount { get; set; }
        public string Payments { get; set; }

        public ClientLoanDTO(ClientLoan clientloan)
        {
            Id = clientloan.Id;
            LoanId = clientloan.Id;
            Name = clientloan.Loan.Name;
            Amount = clientloan.Amount;
            Payments = clientloan.Payments;
        }
    }
}
