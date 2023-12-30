using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CardDeck.Model.Entity
{
    public class TeamDeck
    {
        [Key, Column(Order = 0)]
        public Guid DeckID { get; set; }

        public Guid TeamID { get; set; }
        public DateTime? DeletedAt { get; set; }
    }
}
