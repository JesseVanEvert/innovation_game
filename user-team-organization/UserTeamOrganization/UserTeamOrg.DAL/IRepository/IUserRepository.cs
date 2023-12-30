using System;
using UserTeamOrg.Model.DTO;
using UserTeamOrg.Model.Entity;

namespace UserTeamOrg.DAL.IRepository
{
    public interface IUserRepository
    {
        public Task<string> RegisterUser(UserBaseWithTeamIDDTO userBase, Guid userID);
        public User? GetUser(Guid userId);
        public Task UpdateUser(UserDTO userDTO);
        public Task DeleteUser(Guid userId);
        Task<Guid> LoginUser(string email, string password);
        Task<Dictionary<string, string>> GetRolesAndAccessLevel(Guid personId);
        public Guid GetUserId(string email);
        public void LogoutUser(Guid userId);
        Task<List<UserDTO>> GetUsers(Guid teamId);
        Task DeleteUserFromTeam(Guid userId, Guid teamId);

    }
}

