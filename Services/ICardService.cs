
using HomeBankingAcc.DTOs;

namespace HomeBankingAcc.Services
{
    public interface ICardService
    {
        CardDTO FindById(long id);
        IEnumerable<CardDTO> GetAllCards();
        string genCardNumber();
        int genCvv();
        public CardDTO createCard(long clientId, NewCardDTO newCardDTO);
        public IEnumerable<CardDTO> getClientCards(long clientId);
    }
}
