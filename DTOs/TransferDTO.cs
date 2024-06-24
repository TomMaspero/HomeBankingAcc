namespace HomeBankingAcc.DTOs
{
    public class TransferDTO
    {
        public double Amount { get; set; }
        public string Description { get; set; }
        public string FromAccountNumber { get; set; }
        public string ToAccountNumber { get; set; }
    }
}
