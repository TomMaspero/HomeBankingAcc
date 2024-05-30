using HomeBankingAcc.DTOs;
using HomeBankingAcc.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace HomeBankingAcc.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TransactionsController : Controller
    {
        private readonly ITransactionRepository _transactionRepository;
        public TransactionsController(ITransactionRepository transactionRepository)
        {
            _transactionRepository = transactionRepository;
        }

        [HttpGet]
        public IActionResult GetAllTransactions() 
        {
            try
            {
                var transactions = _transactionRepository.GetAllTransactions();
                var transactionsDTO = transactions.Select(t => new TransactionDTO(t)).ToList();
                return Ok(transactionsDTO);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpGet("{id}")]
        public IActionResult GetTransactionById(long id)
        {
            try
            {
                var transaction = _transactionRepository.FindById(id);
                var transactionDTO = new TransactionDTO(transaction);
                return Ok(transactionDTO);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
    }
}
