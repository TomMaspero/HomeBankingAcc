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
                    return StatusCode(201, newAccount);
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
                Client client = getCurrentClient();
                var clientAccounts = _accountRepository.GetAccountsByClient(client.Id);
                var clientAccountsDTO = clientAccounts.Select(ca => new AccountDTO(ca)).ToList();
                return Ok(clientAccountsDTO);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
        public string genCardNumber()
        {
            string cardNumber = "";
            List<int> cardDigits = new List<int>();
            do
            {
                for (int i = 0; i < 4; i++)
                {
                    int digits = RandomNumberGenerator.GetInt32(1, 9999);
                    cardDigits.Add(digits);
                }
                cardNumber = string.Join("-", cardDigits.ConvertAll(d => d.ToString("D4")));

            } while (_cardRepository.FindByNumber(cardNumber) != null);
            return cardNumber;
        }
        public int genCvv()
        {
            return RandomNumberGenerator.GetInt32(0, 999);
        }

        [HttpPost("current/cards")]
        [Authorize(Policy = "ClientOnly")]
        public IActionResult CreateCard([FromBody] NewCardDTO newCardDTO)
        {
            try
            {
                Client client = getCurrentClient();

                CardType cardType = (CardType)Enum.Parse(typeof(CardType), newCardDTO.type);
                CardColor cardColor = (CardColor)Enum.Parse(typeof(CardColor), newCardDTO.color);

                List<Card> ClientCards = (List<Card>)_cardRepository.FindByType(cardType, client.Id);

                if(ClientCards.Count < 3)
                {
                    if(ClientCards.Any(card => card.Color == cardColor))
                    {
                        return StatusCode(403, "Card already exists");
                    }
                    else
                    {
                        Card newCard = new Card
                        {
                            CardHolder = client.FirstName + " " + client.LastName,
                            Type = cardType,
                            Color = cardColor,
                            Number = genCardNumber(),
                            Cvv = genCvv(),
                            FromDate = DateTime.Now,
                            ThruDate = DateTime.Now.AddYears(5),
                            ClientId = client.Id,
                        };
                        _cardRepository.Save(newCard);
                        return StatusCode(201, new CardDTO(newCard));
                    }
                }
                else
                {
                    return StatusCode(403, $"Max number of cards of type {cardType.ToString()}");
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
                Client client = getCurrentClient();
                var clientCards = _cardRepository.GetCardsByClient(client.Id);
                var clientCardsDTO = clientCards.Select(cc => new CardDTO(cc)).ToList();
                return Ok(clientCardsDTO);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}
