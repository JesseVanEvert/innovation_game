using Azure.Storage.Queues;
using Newtonsoft.Json;
using UserTeamOrg.BLL.Interfaces;
using UserTeamOrg.BLL.Service;
using UserTeamOrg.DAL.Interfaces;
using UserTeamOrg.DAL.Repository;
using UserTeamOrg.Model.DTO;
using UserTeamOrg.Model.Entity;

namespace UserTeamOrg.BLL
{
    public class TeamService : ITeamService
    {
        private ITeamRepository _teamRepository;
        private readonly QueueClient queue;

        public TeamService(ITeamRepository teamRepository)
        {
            _teamRepository = teamRepository;
            queue = QueueService.GetQueueClient();
        }

        public async Task<IEnumerable<Team>> GetTeams(Guid organizationId)
        {
            return await _teamRepository.GetTeams(organizationId);
        }

        public async Task<Team> GetTeam(Guid teamId)
        {
            return await _teamRepository.GetTeam(teamId);
        }

        public async Task<Team> CreateTeam(TeamBaseDTO teamBaseDTO)
        {
            Team team = new()
            {
                Name = teamBaseDTO.Name,
                OrganizationID = teamBaseDTO.OrganizationId,
                TeamID = Guid.NewGuid()
            };

            await _teamRepository.CreateTeam(team);

            /*
                if there were no exceptions when executing the CreateTeam function, a message with the object is send via the queue
                to be put in the replicationdatabase in the Game repository
            */
            KeyValuePair<string, string> queueMessage = new("CreateTeam", JsonConvert.SerializeObject(team));
            await QueueService.AddMessageAsJsonAsync(queueMessage, queue);

            return team;
        }

        public async Task UpdateTeam(TeamBaseWithIdDTO teamBaseWithIdDTO)
        {
            Team team = new()
            {
                Name = teamBaseWithIdDTO.Name,
                OrganizationID = teamBaseWithIdDTO.OrganizationId,
                TeamID = teamBaseWithIdDTO.TeamId
            };

            await _teamRepository.UpdateTeam(team);

            /*
                if there were no exceptions when executing the UpdateTeam function, a message with the object is send via the queue
                to be put in the replicationdatabase in the Game repository
            */
            KeyValuePair<string, string> queueMessage = new("UpdateTeam", JsonConvert.SerializeObject(team));
            await QueueService.AddMessageAsJsonAsync(queueMessage, queue);
        }
        public async Task DeleteTeam(Guid teamId)
        {
            await _teamRepository.DeleteTeam(teamId);

            /*
                if there were no exceptions when executing the DeleteTeam function, a message with the object is send via the queue
                to be put in the replicationdatabase in the Game repository
            */
            KeyValuePair<string, string> queueMessage = new("DeleteTeam", JsonConvert.SerializeObject(teamId));
            await QueueService.AddMessageAsJsonAsync(queueMessage, queue);
        }

        public HashSet<TeamBaseWithIdDTO> GetAllTeamsOfUser(Guid userID)
        {
            return _teamRepository.GetAllTeamsFromOrganization(userID);
        }
    }
}
