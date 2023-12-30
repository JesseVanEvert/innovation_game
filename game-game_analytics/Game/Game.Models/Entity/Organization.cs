using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game.Models.Entity
{
    public class Organization
    {
        public Guid OrganizationID { get; set; }
        public string Name { get; set; }
        public string? ImageUrl { get; set; }
        public DateTime? DeletedAt { get; set; }

        public virtual ICollection<Team> Teams { get; set; }
    }
}
