using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game.Models.Entity
{
    public class Deck
    {
        public Guid DeckID { get; set; }
        public string Name { get; set; }
        public DateTime? DeletedAt { get; set; }

        public virtual ICollection<CardDeck> CardDecks { get; set; }
        public virtual ICollection<TeamDeck> TeamDecks { get; set; }
    }
}
