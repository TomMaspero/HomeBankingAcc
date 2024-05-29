using HomeBankingAcc.Models;
using System.Text.Json.Serialization;

namespace HomeBankingAcc.DTOs
{
    public class TransactionDTO
    {
        public TransactionDTO(Transaction transaction)
        {
            Id = transaction.Id;
            Type = transaction.Type;
            Amount = transaction.Amount;
            Description = transaction.Description;
            Date = transaction.Date;
        }
        [JsonIgnore]
        public long Id { get; set; }
        public TransactionType Type { get; set; }
        public double Amount { get; set; }
        public string Description { get; set; }
        public DateTime Date { get; set; }
    }
}
