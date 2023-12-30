using Game.DAL.Interfaces;
using Game.Exceptions.Exceptions;
using Game.Models.DTO;
using Game.Models.Entity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game.DAL.Repository
{
    public class UserRepository :IUserRepository
    {
        private readonly GameContext _gameContext;

        public UserRepository(GameContext gameContext)
        {
            _gameContext = gameContext; 
        }

        public async Task<User> GetUserAsync(Guid userId)
        {
            return await _gameContext.Users.FindAsync(userId);
        }

        public async Task RegisterUser(User user, UserTeam userTeam)
        {
            await _gameContext.Users.AddAsync(user);
            await _gameContext.UserTeams.AddAsync(userTeam);

            await _gameContext.SaveChangesAsync();
        }

        public async Task UpdateUser(User user)
        {
            User oldUser = await _gameContext.Users
                .Where(u => u.UserID == user.UserID && u.DeletedAt == null)
                .FirstOrDefaultAsync() ?? throw new EntityNotFoundException("Could not find a team with this ID");
            _gameContext.Entry(oldUser).CurrentValues.SetValues(user);
            await _gameContext.SaveChangesAsync();
        }

        public async Task DeleteUser(Guid userId)
        {
            User oldUser = await _gameContext.Users
                .Where(u => u.UserID == userId && u.DeletedAt == null)
                .FirstOrDefaultAsync() ?? throw new EntityNotFoundException("Could not find an organization with this ID");
            User newUser = oldUser;
            newUser.DeletedAt = DateTime.Now;
            _gameContext.Entry(oldUser).CurrentValues.SetValues(newUser);
            await _gameContext.SaveChangesAsync();

            UserTeam oldUserTeam = await _gameContext.UserTeams
                .Where(u => u.UserID == userId && u.DeletedAt == null)
                .FirstOrDefaultAsync() ?? throw new EntityNotFoundException("Could not find an organization with this ID");
            UserTeam newUserTeam = oldUserTeam;
            newUserTeam.DeletedAt = DateTime.Now;
            _gameContext.Entry(oldUser).CurrentValues.SetValues(newUser);
            await _gameContext.SaveChangesAsync();
        }


        public async Task DeleteUserFromTeam(Guid userId, Guid teamId)
        {
            UserTeam? userTeam = await _gameContext.UserTeams
                .Where(ut => ut.UserID == userId && ut.TeamID == teamId && ut.DeletedAt == null)
                .FirstOrDefaultAsync() ?? throw new EntityNotFoundException("Could not find userTeam to delete");

            userTeam.DeletedAt = DateTime.Now;
            await _gameContext.SaveChangesAsync();
        }
    }
}
