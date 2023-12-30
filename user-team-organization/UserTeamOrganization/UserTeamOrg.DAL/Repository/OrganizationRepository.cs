using UserTeamOrg.DAL.Interfaces;
using UserTeamOrg.Model.Entity;
using Microsoft.EntityFrameworkCore;
using UserTeamOrg.Exceptions;

namespace UserTeamOrg.DAL.Repository
{
    public class OrganizationRepository : IOrganizationRepository
    {
        private readonly UserTeamOrganizationContext _userTeamOrganizationContext;
        public OrganizationRepository(UserTeamOrganizationContext UserTeamOrganizationContext)
        {
            _userTeamOrganizationContext = UserTeamOrganizationContext;
        }

        public async Task CreateOrganization(Organization organization)
        {
            await _userTeamOrganizationContext.Organizations.AddAsync(organization);
            await _userTeamOrganizationContext.SaveChangesAsync();
        }

        public async Task<Organization> GetOrganization(Guid organizationId)
        {
            return await _userTeamOrganizationContext.Organizations
                .Where(o => o.OrganizationID == organizationId && o.DeletedAt == null)
                .FirstOrDefaultAsync() ?? throw new NotFoundException("Could not find organization");
        }

        public async Task UpdateOrganization(Organization newOrganization)
        {
            Organization oldOrganization = await _userTeamOrganizationContext.Organizations
                .Where(o => o.OrganizationID == newOrganization.OrganizationID && o.DeletedAt == null)
                .FirstOrDefaultAsync() ?? throw new NotFoundException("Could not find organization");
            _userTeamOrganizationContext.Entry(oldOrganization).CurrentValues.SetValues(newOrganization);

            await _userTeamOrganizationContext.SaveChangesAsync();
        }

        public async Task DeleteOrganization(Guid organizationId)
        {
            Organization oldOrganization = await _userTeamOrganizationContext.Organizations
                .Where(o => o.OrganizationID == organizationId && o.DeletedAt == null)
                .FirstOrDefaultAsync() ?? throw new NotFoundException("Could not find organization");

            Organization newOrganization = oldOrganization;
            newOrganization.DeletedAt = DateTime.Now;
            _userTeamOrganizationContext.Entry(oldOrganization).CurrentValues.SetValues(newOrganization);
            await _userTeamOrganizationContext.SaveChangesAsync();
        }

        public async Task<IEnumerable<Organization>> GetOrganizations()
        {
            return await _userTeamOrganizationContext.Organizations
                .Where(o => o.DeletedAt == null)
                .ToListAsync();
        }
    }
}
