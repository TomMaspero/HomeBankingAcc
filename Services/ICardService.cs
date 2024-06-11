
using HomeBankingAcc.DTOs;

namespace HomeBankingAcc.Services
{
    public interface ICardService
    {
        CardDTO FindById(long id);
        IEnumerable<CardDTO> GetAllCards();
    }
}
