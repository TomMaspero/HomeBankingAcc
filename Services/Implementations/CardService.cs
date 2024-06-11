using HomeBankingAcc.DTOs;
using HomeBankingAcc.Repositories;

namespace HomeBankingAcc.Services.Implementations
{
    public class CardService : ICardService
    {
        private readonly ICardRepository _cardRepository;
        public CardService(ICardRepository cardRepository) 
        {
            _cardRepository = cardRepository;
        }
        public CardDTO FindById(long id)
        {
            var card = _cardRepository.FindById(id);
            var cardDTO = new CardDTO(card);
            return cardDTO;
        }

        public IEnumerable<CardDTO> GetAllCards()
        {
            var cards = _cardRepository.GetAllCards();
            var cardsDTO = cards.Select(c => new CardDTO(c)).ToList();
            return cardsDTO;
        }
    }
}
