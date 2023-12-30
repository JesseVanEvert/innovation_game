using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Game.Models.Entity;

namespace Game.DAL.Interfaces
{
    public interface IOrganizationRepository
    {
        Task CreateOrganization(Organization organization);
        Task UpdateOrganization(Organization organization);
        Task DeleteOrganization(Guid organizationId);
    }
}
