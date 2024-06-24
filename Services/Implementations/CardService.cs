using HomeBankingAcc.DTOs;
using HomeBankingAcc.Models;
using HomeBankingAcc.Repositories;
using Microsoft.VisualBasic;
using System.Security.Cryptography;

namespace HomeBankingAcc.Services.Implementations
{
    public class CardService : ICardService
    {
        private readonly ICardRepository _cardRepository;
        private readonly IClientRepository _clientRepository;
        public CardService(ICardRepository cardRepository, IClientRepository clientRepository) 
        {
            _cardRepository = cardRepository;
            _clientRepository = clientRepository;
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
        public CardDTO createCard(long clientId, NewCardDTO newCardDTO)
        {
            CardType cardType = (CardType)Enum.Parse(typeof(CardType), newCardDTO.type);
            CardColor cardColor = (CardColor)Enum.Parse(typeof(CardColor), newCardDTO.color);

            Client client = _clientRepository.FindById(clientId);
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
            return new CardDTO(newCard);
        }
        public IEnumerable<CardDTO> getClientCards(long clientId) {
            Client client = _clientRepository.FindById(clientId);
            var clientCards = _cardRepository.GetCardsByClient(clientId);
            var clientCardsDTO = clientCards.Select(cc => new CardDTO(cc)).ToList();
            return clientCardsDTO;
        }
    }
}
