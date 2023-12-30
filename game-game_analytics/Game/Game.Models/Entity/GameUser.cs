using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game.Models.Entity
{
    public class GameUser
    {
        public GameUser()
        {
            GameUserAnswers = new HashSet<GameUserAnswer>();    
        }

        [Key, Column(Order = 0)]
        public Guid UserID { get; set; }
        [Key, Column(Order = 1)]
        public Guid GameID { get; set; }
        
        public virtual User User { get; set; }
        public virtual Game Game { get; set; } 

        public DateTime Joined { get; set; }
        public DateTime? Left { get; set; }

        public virtual ICollection<GameUserAnswer>? GameUserAnswers { get; set; }
    }
}
   