﻿using HomeBankingAcc.DTOs;
using HomeBankingAcc.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HomeBankingAcc.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountsController : Controller
    {
        private readonly IAccountRepository _accountRepository;
        public AccountsController(IAccountRepository accountRepository)
        {
            _accountRepository = accountRepository;
        }

        [HttpGet]
        [Authorize(Policy = "AdminOnly")]
        public IActionResult GetAllAccounts()
        {
            try
            {
                var accounts = _accountRepository.GetAllAccounts();
                var accountsDTO = accounts.Select(a => new AccountDTO(a)).ToList();
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
                var account = _accountRepository.FindById(id);
                var accountDTO = new AccountDTO(account);
                return Ok(accountDTO);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
    }
}
