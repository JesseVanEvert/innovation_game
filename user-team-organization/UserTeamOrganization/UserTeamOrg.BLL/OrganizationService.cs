using UserTeamOrg.BLL.Interfaces;
using UserTeamOrg.Model.Entity;
using UserTeamOrg.DAL.Interfaces;
using Newtonsoft.Json;
using UserTeamOrg.BLL.Service;
using Azure.Storage.Queues;
using UserTeamOrg.Model.DTO;

namespace UserTeamOrg.BLL
{
    public class OrganizationService : IOrganizationService
    {
        private IOrganizationRepository _organizationRepository;
        private readonly QueueClient queue;

        public OrganizationService(IOrganizationRepository organizationRepository)
        {
            _organizationRepository = organizationRepository;
            queue = QueueService.GetQueueClient();
        }

        public async Task<IEnumerable<Organization>> GetOrganizations()
        {
            return await _organizationRepository.GetOrganizations();
        }

        public async Task<Organization> GetOrganization(Guid organizationId)
        {
            return await _organizationRepository.GetOrganization(organizationId);
        }

        public async Task<Organization> CreateOrganization(OrganizationBaseDTO organizationBaseDTO)
        {
            Organization organization = new()
            {
                Name = organizationBaseDTO.Name,
                OrganizationID = Guid.NewGuid()
            };

            await _organizationRepository.CreateOrganization(organization);

            /*
                if there were no exceptions when executing the CreateOrganization function, a message with the object is send via the queue
                to be put in the replicationdatabase in the Game repository
            */

            KeyValuePair<string, string> queueMessage = new("CreateOrganization", JsonConvert.SerializeObject(organization));
            await QueueService.AddMessageAsJsonAsync(queueMessage, queue);

            return organization;
        }

        public async Task UpdateOrganization(OrganizationBaseWithIdDTO organizationBaseWithIdDTO)
        {
            Organization organization = new()
            {
                Name = organizationBaseWithIdDTO.Name,
                OrganizationID = organizationBaseWithIdDTO.OrganizationId,
            };

            await _organizationRepository.UpdateOrganization(organization);

            /*
                if there were no exceptions when executing the UpdateOrganization function, a message with the object is send via the queue
                to be updated in the replicationdatabase in the Game repository
            */

            KeyValuePair<string, string> queueMessage = new("UpdateOrganization", JsonConvert.SerializeObject(organization));
            await QueueService.AddMessageAsJsonAsync(queueMessage, queue);
        }

        public async Task DeleteOrganization(Guid organizationId)
        {
            await _organizationRepository.DeleteOrganization(organizationId);

            /*
                if there were no exceptions when executing the DeleteOrganization function, a message with the object is send via the queue
                to be deleted in the replicationdatabase in the Game repository
            */

            KeyValuePair<string, string> queueMessage = new("DeleteOrganization", JsonConvert.SerializeObject(organizationId));
            await QueueService.AddMessageAsJsonAsync(queueMessage, queue);
        }

    }
}
