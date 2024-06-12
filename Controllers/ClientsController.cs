using HomeBankingAcc.DTOs;
using HomeBankingAcc.Models;
using HomeBankingAcc.Repositories;
using HomeBankingAcc.Repositories.Implementations;
using HomeBankingAcc.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.Runtime.Intrinsics.Arm;
using System.Security.Cryptography;

namespace HomeBankingAcc.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClientsController : ControllerBase
    {
        private readonly IClientService _clientService;
        private readonly IAccountService _accountService;
        private readonly ICardService _cardService;
        public ClientsController(IClientService clientService, IAccountService accountService, ICardService cardService)
        {
            _clientService = clientService;
            _accountService = accountService;
            _cardService = cardService;
        }

        [HttpGet]
        [Authorize(Policy = "AdminOnly")]
        public IActionResult GetAllClients()
        {
            try
            {
                var clientsDTO = _clientService.GetAllClients();
                return Ok(clientsDTO);
            }
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, e.Message);
            }
        }

        [HttpGet("{id}")]
        [Authorize(Policy = "AdminOnly")]
        public IActionResult GetClientById(long id) 
        {
            try
            {
                var clientDTO = _clientService.GetClientById(id);
                return Ok(clientDTO);
            }
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, e.Message);
            }
        }

        [HttpPost]
        public IActionResult NewClient([FromBody] NewClientDTO newClientDTO)
        {
            try
            {
                var (isValid, errorMessage) = _clientService.validateClient(newClientDTO);
                if (!isValid)
                {
                    return StatusCode(StatusCodes.Status400BadRequest, errorMessage);
                }
                ClientDTO newClient = _clientService.createClient(newClientDTO);

                return StatusCode(201, newClient);
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

        [HttpGet("current")]
        [Authorize(Policy = "ClientOnly")]
        public IActionResult GetCurrent()
        {
            try
            {
                string currentClientEmail = getCurrentClientEmail();
                var clientDTO = _clientService.getCurrentClientDTO(currentClientEmail);
                return Ok(clientDTO);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost("current/accounts")]
        [Authorize(Policy = "ClientOnly")]
        public IActionResult CreateAccount()
        {
            try
            {
                //traigo al cliente autenticado
                string currentClientEmail = getCurrentClientEmail();
                long currentClientId = _clientService.getCurrentClientDTO(currentClientEmail).Id;

                if(_clientService.validateAccountCount(currentClientId))
                {
                    AccountDTO newAccountDTO = _accountService.createAccount(currentClientId);
                    return StatusCode(201, newAccountDTO);
                }
                else
                {
                    return StatusCode(403, "Max number of accounts reached");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("current/accounts")]
        [Authorize(Policy = "ClientOnly")]
        public IActionResult GetCurrentClientsAccounts()
        {
            try
            {
                string currentClientEmail = getCurrentClientEmail();
                long currentClientId = _clientService.getCurrentClientDTO(currentClientEmail).Id;
                var clientAccountsDTO = _accountService.getClientAccounts(currentClientId);
                return Ok(clientAccountsDTO);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost("current/cards")]
        [Authorize(Policy = "ClientOnly")]
        public IActionResult CreateCard([FromBody] NewCardDTO newCardDTO)
        {
            try
            {
                string currentClientEmail = getCurrentClientEmail();
                long currentClientId = _clientService.getCurrentClientDTO(currentClientEmail).Id;

                var (isValid, errorMessage) = _clientService.validateClientCards(currentClientId,  newCardDTO);
                if (!isValid)
                {
                    return StatusCode(403, errorMessage);
                }
                else
                {
                    CardDTO cardDTO = _cardService.createCard(currentClientId, newCardDTO);
                    return StatusCode(201, cardDTO);
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("current/cards")]
        [Authorize(Policy = "ClientOnly")]
        public IActionResult GetCurrentClientCards()
        {
            try
            {

                string currentClientEmail = getCurrentClientEmail();
                long currentClientId = _clientService.getCurrentClientDTO(currentClientEmail).Id;
                var clientCardsDTO = _cardService.getClientCards(currentClientId);
                return Ok(clientCardsDTO);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}
