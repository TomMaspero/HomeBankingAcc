
using HomeBankingAcc.DTOs;

namespace HomeBankingAcc.Services
{
    public interface ICardService
    {
        CardDTO FindById(long id);
        IEnumerable<CardDTO> GetAllCards();
        //Task<IEnumerable<Card>> GetCardsByClient(long clientId);
        //Task<IEnumerable<Card>> FindByType(CardType type, long clientId);
        //Task<Card> FindByNumber(string number);
        //Task Save(Card card);
    }
}
