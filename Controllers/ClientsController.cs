using HomeBankingAcc.DTOs;
using HomeBankingAcc.Models;
using HomeBankingAcc.Repositories;
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
        private readonly IClientRepository _clientRepository;
        private readonly IAccountRepository _accountRepository;
        private readonly ICardRepository _cardRepository;
        public ClientsController(IClientRepository clientRepository, IAccountRepository accountRepository, ICardRepository cardRepository)
        {
            _clientRepository = clientRepository;
            _accountRepository = accountRepository;
            _cardRepository = cardRepository;
        }

        [HttpGet]
        public IActionResult GetAllClients()
        {
            try
            {
                var clients = _clientRepository.GetAllClients();
                var clientsDTO = clients.Select(c => new ClientDTO(c)).ToList();
                return Ok(clientsDTO);
            }
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, e.Message);
            }
        }

        [HttpGet("{id}")]
        public IActionResult GetClientById(long id)
        {
            try
            {
                var client = _clientRepository.FindById(id);
                var clientDTO = new ClientDTO(client);
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
                if (newClientDTO.FirstName.IsNullOrEmpty() || newClientDTO.LastName.IsNullOrEmpty() || newClientDTO.Email.IsNullOrEmpty() || newClientDTO.Password.IsNullOrEmpty())
                {
                    return StatusCode(StatusCodes.Status400BadRequest, "Missing fields");
                }
                Client client = _clientRepository.FindByEmail(newClientDTO.Email);
                if (client != null)
                {
                    return StatusCode(StatusCodes.Status400BadRequest, "Email already in use");
                }
                Client newClient = new Client
                {
                    FirstName = newClientDTO.FirstName,
                    LastName = newClientDTO.LastName,
                    Email = newClientDTO.Email,
                    Password = newClientDTO.Password,
                };
                _clientRepository.Save(newClient);

                //Crear una nueva cuenta
                Client createdClient = _clientRepository.FindByEmail(newClientDTO.Email);
                Account newAccount = new Account
                {
                    Number = createAccountNumber(),
                    CreationDate = DateTime.Now,
                    Balance = 0,
                    ClientId = createdClient.Id
                };
                _accountRepository.Save(newAccount);

                return StatusCode(201, new ClientDTO(newClientDTO));
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
        public string createAccountNumber()
        {
            string accNumber = "";
            do
            {
                accNumber = "VIN-" + RandomNumberGenerator.GetInt32(0, 99999999);
            } while (_accountRepository.GetAccountByNumber(accNumber) != null);
            return accNumber;
        }

        [HttpGet("current")]
        [Authorize(Policy = "ClientOnly")]
        public IActionResult GetCurrent()
        {
            try
            {
                Client client = getCurrentClient();
                if (client == null)
                {
                    return StatusCode(403, "User not found");
                }
                return Ok(new ClientDTO(client));
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
                Client client = getCurrentClient();
                if (client.Accounts.Count() < 3)
                {
                    Account newAccount = new Account
                    {
                        Number = createAccountNumber(),
                        CreationDate = DateTime.Now,
                        Balance = 0,
                        ClientId = client.Id
                    };
                    _accountRepository.Save(newAccount);
                    return Ok();
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

        public string genCardNumber()
        {
            return "1234";
        }

        public int genCvv()
        {
            return 123;
        }

        [HttpPost("current/cards")]
        [Authorize(Policy = "ClientOnly")]
        public IActionResult CreateCard([FromBody] NewCardDTO newCardDTO)
        {
            try
            {
                Client client = getCurrentClient();

                Card newCard = new Card
                {
                    CardHolder = client.FirstName + " " + client.LastName,
                    Type = (CardType)Enum.Parse(typeof(CardType), newCardDTO.type),
                    Color = (CardColor)Enum.Parse(typeof(CardColor), newCardDTO.color),
                    Number = genCardNumber(),
                    Cvv = genCvv(),
                    FromDate = DateTime.Now,
                    ThruDate = DateTime.Now.AddYears(5),
                    ClientId = client.Id,
                };
                _cardRepository.Save(newCard);
                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}
