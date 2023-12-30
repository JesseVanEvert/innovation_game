using UserTeamOrg.Model.DTO;
using UserTeamOrg.Model.Entity;

namespace UserTeamOrg.BLL.Interfaces
{
    public interface IOrganizationService
    {
        Task<IEnumerable<Organization>> GetOrganizations();
        Task<Organization> GetOrganization(Guid organizationId);
        Task<Organization> CreateOrganization(OrganizationBaseDTO organizationBaseDTO);
        Task UpdateOrganization(OrganizationBaseWithIdDTO organizationBaseWithIdDTO);
        Task DeleteOrganization(Guid organizationId);
    }
}
