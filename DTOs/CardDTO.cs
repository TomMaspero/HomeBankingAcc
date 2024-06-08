using HomeBankingAcc.Models;

namespace HomeBankingAcc.DTOs
{
    public class CardDTO
    {
        public long Id { get; set; }
        public string CardHolder { get; set; }
        public string Type { get; set; }
        public string Color { get; set; }
        public string Number { get; set; }
        public int Cvv { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ThruDate { get; set; }
        public long ClientId { get; set; }
        public CardDTO(Card card)
        {
            Id = card.Id;
            CardHolder = card.CardHolder;
            Type = card.Type.ToString();
            Color = card.Color.ToString();
            Number = card.Number;
            Cvv = card.Cvv;
            FromDate = card.FromDate;
            ThruDate = card.ThruDate;
            ClientId = card.ClientId;
        }
    }
}
