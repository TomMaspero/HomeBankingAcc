using HomeBankingAcc.DTOs;
using HomeBankingAcc.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace HomeBankingAcc.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoansController : Controller
    {
        private readonly ILoanRepository _loanRepository;

        public LoansController(ILoanRepository loanRepository)
        {
            _loanRepository = loanRepository;
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
    }
}
