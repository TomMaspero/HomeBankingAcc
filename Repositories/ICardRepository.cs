using HomeBankingAcc.Models;

namespace HomeBankingAcc.Repositories
{
    public interface ICardRepository
    {
        IEnumerable<Card> GetAllCards();
        void Save(Card card);
        Card FindById(long id);
        Card FindByNumber(string cardNumber);
        IEnumerable<Card> FindByType(CardType type, long clientId);
    }
}
