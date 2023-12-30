namespace CardDeck.Model.Entity
{
    public class Card
    {
        public Guid CardID { get; set; }
        public string FrontSideText { get; set; }
        public string BackSideText { get; set; }  
        public string? ImageUrl { get; set; }
        public DateTime? DeletedAt { get; set; }

        public virtual ICollection<CardDeck>? CardDecks { get; set; }
    }
}
