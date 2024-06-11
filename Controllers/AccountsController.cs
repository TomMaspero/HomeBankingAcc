using HomeBankingAcc.DTOs;
using HomeBankingAcc.Repositories;
using HomeBankingAcc.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HomeBankingAcc.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountsController : Controller
    {
        private readonly IAccountService _accountService;
        public AccountsController(IAccountService accountService)
        {
            _accountService = accountService;
        }

        [HttpGet]
        [Authorize(Policy = "AdminOnly")]
        public IActionResult GetAllAccounts()
        {
            try
            {
                var accountsDTO = _accountService.GetAllAccounts();
                return Ok(accountsDTO);  

            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpGet("{id}")]
        //[Authorize(Policy = "AdminOnly")]
        /*  No le dejo la política de Admin porque el front utiliza el endpoint para acceder a los datos de la cuenta
         *  De lo contrario el front se muestra vacío al acceder a las cuentas de un cliente que no es admin
         */
        public IActionResult GetAccountById(long id) {
            try
            {
                var accountDTO = _accountService.GetAccountById(id);
                return Ok(accountDTO);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
    }
}
