using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UserTeamOrg.Model.Entity
{
    public class UserTeam
    {
        [Key, Column(Order = 0)]
        public Guid UserId { get; set; }

        [Key, Column(Order = 1)]
        public Guid TeamId { get; set; }
        public virtual User User { get; set; }
        public virtual Team Team { get; set; }

        public Role Role { get; set; }
        public DateTime DateJoined { get; set; }
        public DateTime? DeletedAt { get; set; }
    }
}

