using HomeBankingAcc.DTOs;
using HomeBankingAcc.Models;
using HomeBankingAcc.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace HomeBankingAcc.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TransactionsController : Controller
    {
        private readonly ITransactionRepository _transactionRepository;
        private readonly IAccountRepository _accountRepository;
        private readonly IClientRepository _clientRepository;
        public TransactionsController(ITransactionRepository transactionRepository, IAccountRepository accountRepository, IClientRepository clientRepository)
        {
            _transactionRepository = transactionRepository;
            _accountRepository = accountRepository;
            _clientRepository = clientRepository;
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
        [HttpPost]
        [Authorize(Policy = "ClientOnly")]
        public IActionResult CreateTransaction([FromBody] TransferDTO transferDTO)
        {
            try
            {
                if(transferDTO.Amount <= 0 || transferDTO.FromAccountNumber.IsNullOrEmpty() 
                    || transferDTO.ToAccountNumber.IsNullOrEmpty() || transferDTO.Description.IsNullOrEmpty())   
                {
                    return StatusCode(403, "Error: Invalid Transaction");
                }
                if (transferDTO.FromAccountNumber.Equals(transferDTO.ToAccountNumber))
                {
                    return StatusCode(403, "Error: The account of origin cannot be the same as the destination account");
                }
                Account accountFrom = _accountRepository.GetAccountByNumber(transferDTO.FromAccountNumber);
                if(accountFrom == null)
                {
                    return StatusCode(403, "Error: the account of origin does not exist");
                }
                Client client = getCurrentClient();
                if(accountFrom.ClientId != client.Id)
                {
                    return StatusCode(403, "Error: the account of origin does not belong to the current client");
                }
                Account accountTo = _accountRepository.GetAccountByNumber(transferDTO.ToAccountNumber);
                if(accountTo == null)
                {
                    return StatusCode(403, "Error: the destination account does not exist");
                }
                if(accountFrom.Balance - transferDTO.Amount < 0)
                {
                    return StatusCode(403, "Error: Insufficient funds");
                }

                Transaction[] transactions =
                {
                    new Transaction
                    {
                        Type = TransactionType.DEBIT,
                        Amount = -transferDTO.Amount,
                        Description = transferDTO.Description + " - Transfer sent to " + accountTo.Number.ToString(),
                        Date = DateTime.Now,
                        AccountId = accountFrom.Id,
                    },
                    new Transaction
                    {
                        Type = TransactionType.CREDIT,
                        Amount = transferDTO.Amount,
                        Description = transferDTO.Description + " - Transfer received from " + accountFrom.Number.ToString(),
                        Date = DateTime.Now,
                        AccountId = accountTo.Id,
                    }
                };
                TransactionDTO[] transactionDTOs = new TransactionDTO[2];
                int i = 0;
                foreach(var transaction in transactions)
                {
                    _transactionRepository.Save(transaction);
                    TransactionDTO transactionDTO = new TransactionDTO(transaction);
                    transactionDTOs[i] = transactionDTO;
                    i++;
                }

                accountFrom.Balance -= transferDTO.Amount;
                _accountRepository.Save(accountFrom);
                accountTo.Balance += transferDTO.Amount;
                _accountRepository.Save(accountTo);

                return StatusCode(201, transactionDTOs);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
        public Client getCurrentClient()
        {
            string email = User.FindFirst("Client") != null ? User.FindFirst("Client").Value : string.Empty;
            if (email == string.Empty)
            {
                throw new Exception("User not found");
            }
            return _clientRepository.FindByEmail(email);
        }
    }
}
