using HomeBankingAcc.DTOs;
using HomeBankingAcc.Models;

namespace HomeBankingAcc.Services
{
    public interface IClientService
    {
        ClientDTO GetClientById(long id);
        IEnumerable<ClientDTO> GetAllClients();
        Client getCurrentClient(string email);
        ClientDTO getCurrentClientDTO(string email);
        (bool isValid, string errorMessage) validateClient(NewClientDTO newClientDTO);
        ClientDTO createClient(NewClientDTO clientDTO);
        bool validateAccountCount(long clientId);
        public (bool isValid, string errorMessage) validateClientCards(long clientId, NewCardDTO newCardDTO);
    }
}
