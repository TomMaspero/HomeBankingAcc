﻿using HomeBankingAcc.Models;

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
        public IEnumerable<Card> GetCardsByClient(long clientId)
        {
            return FindByCondition(c => c.ClientId == clientId)
                .ToList();
        }
        public IEnumerable<Card> FindByType(CardType type, long clientId)
        {
            return FindByCondition(c => c.Type == type && c.ClientId == clientId).ToList();
        }
        public Card FindByNumber(string cardNumber)
        {
            return FindByCondition(c => c.Number == cardNumber)
                .FirstOrDefault();
        }
        public void Save(Card card)
        {
            Create(card);
            SaveChanges();
        }
    }
}
