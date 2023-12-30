using UserTeamOrg.DAL.Interfaces;
using UserTeamOrg.Model.Entity;
using Microsoft.EntityFrameworkCore;
using UserTeamOrg.Exceptions;
using UserTeamOrg.Model.DTO;

namespace UserTeamOrg.DAL.Repository
{
    public class TeamRepository : ITeamRepository
    {
        private readonly UserTeamOrganizationContext _userTeamOrganizationContext;
        public TeamRepository(UserTeamOrganizationContext UserTeamOrganizationContext)
        {
            _userTeamOrganizationContext = UserTeamOrganizationContext;
        }

        public async Task CreateTeam(Team team)
        {
            await _userTeamOrganizationContext.Teams.AddAsync(team);
            await _userTeamOrganizationContext.SaveChangesAsync();
        }

        public async Task<Team> GetTeam(Guid teamId)
        {
            return await _userTeamOrganizationContext.Teams
                .Where(t => t.TeamID == teamId && t.DeletedAt == null)
                .FirstOrDefaultAsync() ?? throw new NotFoundException("Could not find team");
        }

        public async Task UpdateTeam(Team newTeam)
        {
            Team oldTeam = await _userTeamOrganizationContext.Teams
                .Where(t => t.TeamID == newTeam.TeamID && t.DeletedAt == null)
                .FirstOrDefaultAsync() ?? throw new NotFoundException("Could not find team");
            _userTeamOrganizationContext.Entry(oldTeam).CurrentValues.SetValues(newTeam);

            await _userTeamOrganizationContext.SaveChangesAsync();
        }

        public async Task DeleteTeam(Guid teamId)
        {
            Team oldTeam = await _userTeamOrganizationContext.Teams
                .Where(t => t.TeamID == teamId && t.DeletedAt == null)
                .FirstOrDefaultAsync() ?? throw new NotFoundException("Could not find team");

            Team newTeam = oldTeam;
            oldTeam.DeletedAt = DateTime.Now;
            _userTeamOrganizationContext.Entry(oldTeam).CurrentValues.SetValues(newTeam);
            await _userTeamOrganizationContext.SaveChangesAsync();
        }

        public HashSet<TeamBaseWithIdDTO> GetAllTeamsFromOrganization(Guid userId) {
            IEnumerable<Team> teams = from users in _userTeamOrganizationContext.Users
                        where users.UserId == userId
                       from userTeam in _userTeamOrganizationContext.UserTeams
                        where userTeam.UserId == userId
                       from team in _userTeamOrganizationContext.Teams
                        where team.TeamID == userTeam.TeamId
                       select team;

            HashSet<TeamBaseWithIdDTO> teamsDTO = new();

            foreach(Team team in teams)
            {
                teamsDTO.Add(new()
                {
                    Name = team.Name,
                    OrganizationId = team.OrganizationID,
                    TeamId = team.TeamID
                });
            }

            return teamsDTO;
        }

        //Get all teams belonging to an organization
        public async Task<IEnumerable<Team>> GetTeams(Guid organizationId)
        {
            return await _userTeamOrganizationContext.Teams
                .Where(t => t.OrganizationID == organizationId && t.DeletedAt == null)
                .ToListAsync();
        }
    }
}
