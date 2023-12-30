using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game.Models.Entity
{
    public class UserTeam
    {
        [Key, Column(Order = 0)]
        public Guid UserID { get; set; }
        [Key, Column(Order = 1)]
        public Guid TeamID { get; set; }
        
        public virtual User User { get; set; }
        public virtual Team Team { get; set; }  

        public DateTime DateJoined { get; set; }
        public DateTime? DeletedAt { get; set; }
    }
}
