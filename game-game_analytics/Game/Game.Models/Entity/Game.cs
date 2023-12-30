using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game.Models.Entity
{
    public class Game
    {
        public Game()
        {
            GameUsers = new HashSet<GameUser>();
        }

        public Guid GameID { get; set; }
        public string? Name { get; set; }
        public DateTime Start { get; set; }
        public DateTime? End { get; set; }
        public DateTime? DeletedAt { get; set; }

        public virtual ICollection<GameUser>? GameUsers { get; set; }
    }
}
