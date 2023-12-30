using UserTeamOrg.Model.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace UserTeamOrg.BLL.Interfaces
{
    public interface IUserService
    {
        Task<string> RegisterUser(UserBaseWithTeamIDDTO userBase);
        UserDTO GetUser(Guid userId);
        Task UpdateUser(UserDTO userBase);
        Task DeleteUser(Guid userId);
        Task<Guid> LoginUser(string email, string password);
        Task<Dictionary<string, string>> GetRolesAndAccessLevel(Guid personId);
        Guid GetUserId(string email);
        void LogoutUser(Guid userId);
        Task<List<UserDTO>> GetUsers(Guid teamId);
        Task DeleteUserFromTeam(UserTeamDTO userTeam);
    }
}

