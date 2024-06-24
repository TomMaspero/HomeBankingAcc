using HomeBankingAcc.DTOs;
using HomeBankingAcc.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HomeBankingAcc.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoansController : Controller
    {
        private readonly ILoanService _loanService;
        private readonly IClientService _clientService;
        public LoansController(ILoanService loanService, IClientService clientService)
        {
            _loanService = loanService;
            _clientService = clientService;
        }

        [HttpGet]
        public IActionResult GetAllLoans()
        {
            try
            {
                var loansDTO = _loanService.GetAllLoans();
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
                var loanDTO = _loanService.GetLoanById(id);
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
                string currentClientEmail = getCurrentClientEmail();
                long currentClientId = _clientService.getCurrentClientDTO(currentClientEmail).Id;

                var (isValid, errorMessage) = _loanService.validateLoan(currentClientId, loanApplicationDTO);

                if (!isValid)
                    return StatusCode(403, errorMessage);

                var newClientLoanDTO = _loanService.createLoan(currentClientId, loanApplicationDTO);

                return Ok(newClientLoanDTO);
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
