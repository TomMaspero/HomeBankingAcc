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
    public class LoansController : Controller
    {
        private readonly IClientRepository _clientRepository;
        private readonly IAccountRepository _accountRepository;
        private readonly ILoanRepository _loanRepository;
        private readonly IClientLoanRepository _clientLoanRepository;
        private readonly ITransactionRepository _transactionRepository;

        public LoansController(ILoanRepository loanRepository, IClientRepository clientRepository, IAccountRepository accountRepository, IClientLoanRepository clientLoanRepository, ITransactionRepository transactionRepository)
        {
            _loanRepository = loanRepository;
            _clientRepository = clientRepository;
            _accountRepository = accountRepository;
            _clientLoanRepository = clientLoanRepository;
            _transactionRepository = transactionRepository;
        }

        [HttpGet]
        public IActionResult GetAllLoans()
        {
            try
            {
                var loans = _loanRepository.GetAllLoans();
                var loansDTO = loans.Select(l => new LoanDTO(l)).ToList();
                return Ok(loansDTO);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpGet("{id}")]
        public IActionResult GetLoanById(long id)
        {
            try
            {
                var loan = _loanRepository.FindById(id);
                var loanDTO = new LoanDTO(loan);
                return Ok(loanDTO);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpPost]
        [Authorize (Policy = "ClientOnly")]
        public IActionResult CreateLoan([FromBody] LoanApplicationDTO loanApplicationDTO)
        {
            try
            {
                Loan loan = _loanRepository.FindById(loanApplicationDTO.LoanId);
                Account account = _accountRepository.GetAccountByNumber(loanApplicationDTO.ToAccountNumber);
                Client client = getCurrentClient();
                if(loanApplicationDTO.Amount <= 0) 
                {
                    return StatusCode(403, "Error: invalid loan amount");
                }
                if(loan == null)
                {
                    return StatusCode(403, "Error: loan does not exist!");
                }
                if(loanApplicationDTO.Amount > loan.MaxAmount)
                {
                    return StatusCode(403, "Error: loan cannot exceed the maximum amount");
                }
                if(loanApplicationDTO.Payments.IsNullOrEmpty())
                {
                    return StatusCode(403, "Error: Payments cannot be empty");
                }
                if(account == null)
                {
                    return StatusCode(403, "Error: the account does not exist");
                }
                if (!client.Accounts.Any(acc => acc.Number.Equals(account.Number)))
                {
                    return StatusCode(403, "Error: the account does not belong to the current client");
                }

                // Actualizar el balance de la cuenta

                ClientLoan newClientLoan = new ClientLoan
                {
                    Amount = loanApplicationDTO.Amount * 1.2,
                    Payments = loanApplicationDTO.Payments,
                    ClientId = client.Id,
                    LoanId = loanApplicationDTO.LoanId,
                };
                _clientLoanRepository.Save(newClientLoan);

                Transaction loanTransaction = new Transaction
                {
                    Type = TransactionType.CREDIT,
                    Amount = loanApplicationDTO.Amount,
                    Description = "Loan: " + loan.Name + " to Account " + account.Number,
                    Date = DateTime.Now,
                    AccountId = account.Id,
                };
                _transactionRepository.Save(loanTransaction);

                account.Balance += loanApplicationDTO.Amount;
                _accountRepository.Save(account);
                
                return Ok(new NewClientLoanDTO(newClientLoan, loan,loanTransaction));
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
