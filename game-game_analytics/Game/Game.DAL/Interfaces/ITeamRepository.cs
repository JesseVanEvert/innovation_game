using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Game.Models.Entity;

namespace Game.DAL.Interfaces
{
    public interface ITeamRepository
    {
        Task CreateTeam(Team team);
        Task UpdateTeam(Team team);
        Task DeleteTeam(Guid teamId);
    }
}
