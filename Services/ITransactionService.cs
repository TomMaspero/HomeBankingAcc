using HomeBankingAcc.DTOs;

namespace HomeBankingAcc.Services
{
    public interface ITransactionService
    {
        TransactionDTO GetTransactionById(long id);
        IEnumerable<TransactionDTO> GetAllTransactions();
        public (bool isValid, string errorMessage) validateTransaction(long clientId, TransferDTO transferDTO);
        public TransactionDTO[] createTransaction(TransferDTO transferDTO);
    }
}
