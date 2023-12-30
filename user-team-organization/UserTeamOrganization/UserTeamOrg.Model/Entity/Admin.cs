using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace UserTeamOrg.Model.Entity
{
    public class Admin
    {
        public Guid AdminId { get; set; }
        public AccessLevel accessLevel { get; set; }
        public Guid PersonId { get; set; }
        public DateTime? DeletedAt { get; set; }

        public Person Person { get; set; }

    }
}

