using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game.Models.Entity
{
    public class Card
    {
        public Guid CardID { get; set; }   
        public string FrontSideText { get; set; }
        public string BackSideText { get; set; }
        public string? ImageUrl { get; set; }
        public DateTime? DeletedAt { get; set; }

        public virtual ICollection<GameUserScore> GameUserScores { get; set; }
        public virtual ICollection<CardDeck> CardDecks { get; set; }
    }
}
