using Game.DAL;
using Game.DAL.Interfaces;
using Game.Exceptions.Exceptions;
using Game.Models.Entity;
using Microsoft.EntityFrameworkCore;
using System.Runtime.Intrinsics.Arm;

namespace UserTeamOrg.DAL.Repository
{
    public class OrganizationRepository : IOrganizationRepository
    {
        private readonly GameContext _gameContext;
        public OrganizationRepository(GameContext gameContext)
        {
            _gameContext = gameContext;
        }

        public async Task CreateOrganization(Organization organization)
        {
            await _gameContext.Organizations.AddAsync(organization);
            await _gameContext.SaveChangesAsync();
        }

        public async Task UpdateOrganization(Organization newOrganization)
        {
            Organization oldOrganization = await _gameContext.Organizations
                .Where(o => o.OrganizationID == newOrganization.OrganizationID && o.DeletedAt == null)
                .FirstOrDefaultAsync() ?? throw new EntityNotFoundException("Could not find an organization with this ID");
            _gameContext.Entry(oldOrganization).CurrentValues.SetValues(newOrganization);

            await _gameContext.SaveChangesAsync();

        }

        public async Task DeleteOrganization(Guid organizationId)
        {
            Organization oldOrganization = await _gameContext.Organizations
                .Where(o => o.OrganizationID == organizationId && o.DeletedAt == null)
                .FirstOrDefaultAsync() ?? throw new EntityNotFoundException("Could not find an organization with this ID");
            Organization newOrganization = oldOrganization;
            newOrganization.DeletedAt = DateTime.Now;
            _gameContext.Entry(oldOrganization).CurrentValues.SetValues(newOrganization);
            await _gameContext.SaveChangesAsync();
        }

    }
}
