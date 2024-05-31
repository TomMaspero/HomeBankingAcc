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
        public DateTime FormDate { get; set; }
        public DateTime ThruDate { get; set; }

        public CardDTO(Card card)
        {
            Id = card.Id;
            CardHolder = card.CardHolder;
            Type = card.Type.ToString();
            Color = card.Color.ToString();
            Number = card.Number;
            Cvv = card.Cvv;
            FormDate = card.FormDate;
            ThruDate = card.ThruDate;
        }
    }
}
