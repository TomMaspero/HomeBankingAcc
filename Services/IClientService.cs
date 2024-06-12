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
        string createAccountNumber();
        string genCardNumber();
        int genCvv();
    }
}
