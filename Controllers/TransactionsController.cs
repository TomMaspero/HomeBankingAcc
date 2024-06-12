using HomeBankingAcc.DTOs;
using HomeBankingAcc.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HomeBankingAcc.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TransactionsController : Controller
    {
        private readonly ITransactionService _transactionService;
        private readonly IClientService _clientService;
        public TransactionsController(ITransactionService transactionService, IClientService clientService)
        {
            _transactionService = transactionService;
            _clientService = clientService;
        }

        [HttpGet]
        [Authorize(Policy = "AdminOnly")]
        public IActionResult GetAllTransactions() 
        {
            try
            {
                var transactionsDTO = _transactionService.GetAllTransactions();
                return Ok(transactionsDTO);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpGet("{id}")]
        [Authorize(Policy = "AdminOnly")]
        public IActionResult GetTransactionById(long id)
        {
            try
            {
                var transactionDTO = _transactionService.GetTransactionById(id);
                return Ok(transactionDTO);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
        [HttpPost]
        [Authorize(Policy = "ClientOnly")]
        public IActionResult CreateTransaction([FromBody] TransferDTO transferDTO)
        {
            try
            {
                string currentClientEmail = getCurrentClientEmail();
                long currentClientId = _clientService.getCurrentClientDTO(currentClientEmail).Id;

                var (isValid, errorMessage) = _transactionService.validateTransaction(currentClientId, transferDTO);
                if (!isValid)
                    return StatusCode(403, errorMessage);

                var transactionDTOs = _transactionService.createTransaction(transferDTO);

                return StatusCode(201, transactionDTOs);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
        public string getCurrentClientEmail()
        {
            string currentClientEmail = User.FindFirst("Client") != null ? User.FindFirst("Client").Value : string.Empty;
            return currentClientEmail;
        }

    }
}
