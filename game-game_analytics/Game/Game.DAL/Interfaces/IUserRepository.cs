using System;
using Game.Models.DTO;
using Game.Models.Entity;

namespace Game.DAL.Interfaces
{
    public interface IUserRepository
    {
        Task<User> GetUserAsync(Guid userId);
        Task RegisterUser(User user, UserTeam userTeam);
        Task UpdateUser(User user);
        Task DeleteUser(Guid userId);
        Task DeleteUserFromTeam(Guid userId, Guid teamId);
    }
}

