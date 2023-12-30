using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game.Models.Entity
{
    public class User
    {
        public Guid UserID { get; set; }
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public string Email { get; set; }
        public string? ImageUrl { get; set; }
        public DateTime? DeletedAt { get; set; }

        public virtual ICollection<GameUser> GameUsers { get; set; }
        public virtual ICollection<UserTeam> UserTeams { get; set; }
    }
}
