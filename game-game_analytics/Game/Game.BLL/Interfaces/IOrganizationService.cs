using Game.Models.Entity;

namespace Game.BLL.Interfaces
{
    public interface IOrganizationService
    {
        Task CreateOrganization(Organization organization);
        Task UpdateOrganization(Organization organization);
        Task DeleteOrganization(Guid organizationId);
    }
}
