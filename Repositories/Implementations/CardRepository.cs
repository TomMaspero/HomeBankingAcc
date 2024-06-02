using HomeBankingAcc.Models;

namespace HomeBankingAcc.Repositories.Implementations
{
    public class CardRepository : RepositoryBase<Card>, ICardRepository
    {
        public CardRepository(HomeBankingContext repositoryContext) : base(repositoryContext)
        {
        }

        public Card FindById(long id)
        {
            return FindByCondition(c => c.Id == id)
                .FirstOrDefault();
        }

        public IEnumerable<Card> GetAllCards()
        {
            return FindAll().ToList();
        }

        public void Save(Card card)
        {
            Create(card);
            SaveChanges();
        }
    }
}
