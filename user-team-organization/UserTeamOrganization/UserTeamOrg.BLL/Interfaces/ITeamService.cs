using UserTeamOrg.Model.DTO;
using UserTeamOrg.Model.Entity;

namespace UserTeamOrg.BLL.Interfaces
{
    public interface ITeamService
    {
        //void GetAllTeams();
        Task<Team> GetTeam(Guid teamId);
        Task<Team> CreateTeam(TeamBaseDTO teamBaseDTO);
        Task UpdateTeam(TeamBaseWithIdDTO teamBaseWithIdDTO);
        Task DeleteTeam(Guid teamId);
        HashSet<TeamBaseWithIdDTO> GetAllTeamsOfUser(Guid userID);
        Task<IEnumerable<Team>> GetTeams(Guid organizationId);
    }
}
