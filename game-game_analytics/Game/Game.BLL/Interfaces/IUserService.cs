using Game.Models.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Game.BLL.Interfaces
{
    public interface IUserService
    {
        Task RegisterUser(UserBaseWithIDAndTeamIDDTO userBase);
        Task UpdateUser(UserDTO userBase);
        Task DeleteUser(Guid userId);
        Task DeleteUserFromTeam(UserTeamDTO userTeam);
    }
}

