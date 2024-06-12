using HomeBankingAcc.DTOs;
using HomeBankingAcc.Models;
using HomeBankingAcc.Repositories;
using System.Security.Cryptography;

namespace HomeBankingAcc.Services.Implementations
{
    public class ClientService : IClientService
    {
        private readonly IClientRepository _clientRepository;
        private readonly IAccountRepository _accountRepository;
        private readonly ICardRepository _cardRepository;
        public ClientService(IClientRepository clientRepository, IAccountRepository accountRepository, ICardRepository cardRepository)
        {
            _clientRepository = clientRepository;
            _accountRepository = accountRepository;
            _cardRepository = cardRepository;
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
        public string createAccountNumber()
        {
            string accNumber = "";
            do
            {
                accNumber = "VIN-" + RandomNumberGenerator.GetInt32(0, 99999999);
            } while (_accountRepository.GetAccountByNumber(accNumber) != null);
            return accNumber;
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

        public ClientDTO getCurrent()
        {

        }
        public ClientDTO getCurrentClient(string email)
        {
            if(email == null)
            {
                throw new Exception("User not found");
            }
            var client = _clientRepository.FindByEmail(email);
            var clientDTO = new ClientDTO(client);
            return clientDTO;
        }
    }
}
