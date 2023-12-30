using Game.BLL.Interfaces;
using Game.Models.Entity;
using Game.DAL.Interfaces;
using Newtonsoft.Json;
using Game.BLL.Service;
using Azure.Storage.Queues;

namespace Game.BLL
{
    public class OrganizationService : IOrganizationService
    {
        private IOrganizationRepository _organizationRepository;

        public OrganizationService(IOrganizationRepository organizationRepository)
        {
            _organizationRepository = organizationRepository;
        }

        public async Task CreateOrganization(Organization organization)
        {
            await _organizationRepository.CreateOrganization(organization);
        }

        public async Task UpdateOrganization(Organization organization) 
        {
            await _organizationRepository.UpdateOrganization(organization);
        }

        public async Task DeleteOrganization(Guid organizationId)
        {
            await _organizationRepository.DeleteOrganization(organizationId);
        }

    }
}
