using HomeBankingAcc.Models;

namespace HomeBankingAcc.Repositories
{
    public interface ICardRepository
    {
        IEnumerable<Card> GetAllCards();
        IEnumerable<Card> GetCardsByClient(long clientId);
        IEnumerable<Card> FindByType(CardType type, long clientId);
        Card FindById(long id);
        Card FindByNumber(string cardNumber);
        void Save(Card card);
    }
}
