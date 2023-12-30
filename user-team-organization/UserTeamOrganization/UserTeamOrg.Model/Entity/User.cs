using System;
namespace UserTeamOrg.Model.Entity
{
    public class User
    {
        public Guid UserId { get; set; }
        public Person Person { get; set; }        
        public Guid PersonId { get; set; }
        public DateTime? DeletedAt { get; set; }


        public virtual ICollection<UserTeam> UserTeams {get;set;}
    }
}

