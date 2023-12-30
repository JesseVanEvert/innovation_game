using Azure.Storage.Queues;
using Game.BLL.Interfaces;
using Game.DAL.Interfaces;
using Game.Models.Entity;
using Newtonsoft.Json;

namespace Game.BLL
{
    public class TeamService : ITeamService
    {
        private ITeamRepository _teamRepository;

        public TeamService(ITeamRepository teamRepository)
        {
            _teamRepository = teamRepository;
        }

        public async Task CreateTeam(Team team)
        {
            await _teamRepository.CreateTeam(team);
        }

        public async Task UpdateTeam(Team team)
        {
            await _teamRepository.UpdateTeam(team);
        }
        public async Task DeleteTeam(Guid teamId)
        {
            await _teamRepository.DeleteTeam(teamId);
        }
    }
}
