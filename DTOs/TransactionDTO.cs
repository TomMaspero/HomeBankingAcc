using HomeBankingAcc.Models;
using Microsoft.Identity.Client;
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
            AccountId = transaction.AccountId;
        }
        public long Id { get; set; }
        public TransactionType Type { get; set; }
        public double Amount { get; set; }
        public string Description { get; set; }
        public DateTime Date { get; set; }
        public long AccountId { get; set; }
    }
}
