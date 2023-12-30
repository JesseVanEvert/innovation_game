using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserTeamOrg.Model.Entity;

namespace UserTeamOrg.DAL.Interfaces
{
    public interface IOrganizationRepository
    {
        //void GetAllOrganizations();
        Task<Organization> GetOrganization(Guid organizationId);
        Task CreateOrganization(Organization organization);
        Task UpdateOrganization(Organization organization);
        Task DeleteOrganization(Guid organizationId);
        Task<IEnumerable<Organization>> GetOrganizations();
    }
}
