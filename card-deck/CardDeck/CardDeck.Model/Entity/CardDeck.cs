using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CardDeck.Model.Entity
{
    public class CardDeck
    {
        [Key, Column(Order = 1)]
        public Guid CardID { get; set; }

        [Key, Column(Order = 0)]
        public Guid DeckID { get; set; }
        public DateTime? DeletedAt { get; set; }
    }
}