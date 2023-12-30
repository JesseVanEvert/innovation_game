using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserTeamOrg.Model.DTO;
using UserTeamOrg.Model.Entity;

namespace UserTeamOrg.DAL.Interfaces
{
    public interface ITeamRepository
    {
        Task<Team> GetTeam(Guid teamId);
        Task CreateTeam(Team team);
        Task UpdateTeam(Team team);
        Task DeleteTeam(Guid teamId);
        HashSet<TeamBaseWithIdDTO> GetAllTeamsFromOrganization(Guid userId);
        Task<IEnumerable<Team>> GetTeams(Guid organizationId);
    }
}
