using HomeBankingAcc.Models;

namespace HomeBankingAcc.DTOs
{
    public class NewClientLoanDTO
    {
        public long LoanId { get; set; }
        public long ClientId { get; set; }
        public string Name { get; set; }
        public double Amount { get; set; }
        public string Payments { get; set; }
        public string ToAccountNumber { get; set; }
        public TransactionDTO Transaction { get; set; }

        public NewClientLoanDTO(ClientLoan clientLoan, Loan loan,Transaction transaction)
        {
            LoanId = clientLoan.LoanId;
            ClientId = clientLoan.ClientId;
            Name = loan.Name;
            Amount = clientLoan.Amount;
            Payments = clientLoan.Payments;
            ToAccountNumber = transaction.Account.Number;
            Transaction = new TransactionDTO(transaction);
        }
    }
}
