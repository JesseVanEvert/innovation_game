using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game.Models.Entity
{
    public class TeamDeck
    {
        [Key, Column(Order = 0)]
        public Guid DeckID { get; set; }
        [Key, Column(Order = 1)]
        public Guid TeamID { get; set; }
        public DateTime? DeletedAt { get; set; }
    }
}
