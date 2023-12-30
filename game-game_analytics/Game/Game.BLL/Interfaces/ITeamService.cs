using Game.Models.Entity;

namespace Game.BLL.Interfaces
{
    public interface ITeamService
    {
        Task CreateTeam(Team team);
        Task UpdateTeam(Team team);
        Task DeleteTeam(Guid teamId);
    }
}
