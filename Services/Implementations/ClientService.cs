using HomeBankingAcc.DTOs;
using HomeBankingAcc.Models;
using HomeBankingAcc.Repositories;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System.Security.Cryptography;

namespace HomeBankingAcc.Services.Implementations
{
    public class ClientService : IClientService
    {
        private readonly IClientRepository _clientRepository;
        private readonly IAccountRepository _accountRepository;
        private readonly ICardRepository _cardRepository;
        private readonly IAccountService _accountService;
        private readonly ICardService _cardService;
        public ClientService(IClientRepository clientRepository, IAccountRepository accountRepository, ICardRepository cardRepository, IAccountService accountService, ICardService cardService)
        {
            _clientRepository = clientRepository;
            _accountRepository = accountRepository;
            _cardRepository = cardRepository;
            _accountService = accountService;
            _cardService = cardService;
        }
        public IEnumerable<ClientDTO> GetAllClients()
        {
            var clients = _clientRepository.GetAllClients();
            var clientsDTO = clients.Select(c => new ClientDTO(c)).ToList();
            return clientsDTO;
        }

        public ClientDTO GetClientById(long id)
        {
            var client = _clientRepository.FindById(id);
            var clientDTO = new ClientDTO(client);
            return clientDTO;
        }
        public Client getCurrentClient(string email)
        {
            if (email == null)
            {
                throw new Exception("User not found");
            }
            var client = _clientRepository.FindByEmail(email);
            return client;
        }
        public ClientDTO getCurrentClientDTO(string email)
        {
            var client = getCurrentClient(email);
            var clientDTO = new ClientDTO(client);
            return clientDTO;
        }
        public (bool isValid, string errorMessage) validateClient(NewClientDTO newClientDTO)
        {
            if (string.IsNullOrWhiteSpace(newClientDTO.FirstName))
                return (false, "First name is required.");
            if (string.IsNullOrWhiteSpace(newClientDTO.LastName))
                return (false, "Last name is required.");
            if (string.IsNullOrWhiteSpace(newClientDTO.Email))
                return (false, "Email is required.");
            if (string.IsNullOrWhiteSpace(newClientDTO.Password))
                return (false, "Password is required.");

            Client existingClient = _clientRepository.FindByEmail(newClientDTO.Email);
            if (existingClient != null)
                return (false, "Email already in use.");

            return (true, string.Empty);
        }
        public ClientDTO createClient(NewClientDTO newClientDTO)
        {
            Client newClient = new Client
            {
                FirstName = newClientDTO.FirstName,
                LastName = newClientDTO.LastName,
                Email = newClientDTO.Email,
                Password = HashPassword(newClientDTO.Password),
            };
            _clientRepository.Save(newClient);

            Client createdClient = _clientRepository.FindByEmail(newClientDTO.Email);

            Account newAccount = new Account
            {
                Number = _accountService.createAccountNumber(),
                CreationDate = DateTime.Now,
                Balance = 0,
                ClientId = createdClient.Id
            };
            _accountRepository.Save(newAccount);

            return new ClientDTO(newClientDTO);
        }
        public bool validateAccountCount(long clientId)
        {
            Client client = _clientRepository.FindById(clientId);
            return client.Accounts.Count() < 3 ? true : false;
        }
        public (bool isValid, string errorMessage) validateClientCards(long clientId, NewCardDTO newCardDTO)
        {
            CardType cardType = (CardType)Enum.Parse(typeof(CardType), newCardDTO.type);
            CardColor cardColor = (CardColor)Enum.Parse(typeof(CardColor), newCardDTO.color);

            List<Card> ClientCards = (List<Card>)_cardRepository.FindByType(cardType, clientId);

            if (ClientCards.Count < 3)
            {
                if (ClientCards.Any(card => card.Color == cardColor))
                    return (false, "Card already exists");
                else
                    return (true, string.Empty);
            }
            else
                return (false, $"Max number of cards of type {cardType.ToString()}");
        }
        public (bool isValid, string errorMessage) validateClientUser(LoginDTO loginDTO)
        {
            Client user = _clientRepository.FindByEmail(loginDTO.Email);
            if (user == null)
                return (false, "User not found");
            if (!user.Password.Equals(HashPassword(loginDTO.Password)))
                return (false, "Invalid password");
            return (true, string.Empty);
        }
        public string HashPassword(string password)
        {
            byte[] salt = RandomNumberGenerator.GetBytes(128 / 8); // divide by 8 to convert bits to bytes
            
            // derive a 256-bit subkey (use HMACSHA256 with 100,000 iterations)
            string hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password: password!,
                salt: salt,
                prf: KeyDerivationPrf.HMACSHA256,
                iterationCount: 100000,
                numBytesRequested: 256 / 8));
            return hashed;
        }
    }
}
