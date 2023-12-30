using Game.DAL;
using Game.DAL.Interfaces;
using Game.Exceptions.Exceptions;
using Game.Models.Entity;
using Microsoft.EntityFrameworkCore;

namespace UserTeamOrg.DAL.Repository
{
    public class TeamRepository : ITeamRepository
    {
        private readonly GameContext _gameContext;
        public TeamRepository(GameContext gameContext)
        {
            _gameContext = gameContext;
        }

        public async Task CreateTeam(Team team)
        {
            await _gameContext.Teams.AddAsync(team);
            await _gameContext.SaveChangesAsync();
        }
        public async Task UpdateTeam(Team newTeam)
        {
            Team oldTeam = await _gameContext.Teams
                .Where(t => t.TeamID == newTeam.TeamID && t.DeletedAt == null)
                .FirstOrDefaultAsync() ?? throw new EntityNotFoundException("Could not find a team with this ID");
            _gameContext.Entry(oldTeam).CurrentValues.SetValues(newTeam);
            await _gameContext.SaveChangesAsync();
        }

        public async Task DeleteTeam(Guid teamId)
        {
            Team oldTeam = await _gameContext.Teams
                .Where(t => t.TeamID == teamId && t.DeletedAt == null)
                .FirstOrDefaultAsync() ?? throw new EntityNotFoundException("Could not find an organization with this ID");
            Team newTeam = oldTeam;
            newTeam.DeletedAt = DateTime.Now;
            _gameContext.Entry(oldTeam).CurrentValues.SetValues(newTeam);
            await _gameContext.SaveChangesAsync();
        }

    }
}
