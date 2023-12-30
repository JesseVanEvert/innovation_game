using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UserTeamOrg.Model.DTO;
using UserTeamOrg.Model.Entity;
using UserTeamOrg.DAL.IRepository;
using UserTeamOrg.Exceptions;
using System.Security.Cryptography;
using System.Text;

namespace UserTeamOrg.DAL.Repository
{
    public class UserRepository : IUserRepository
    {
        private readonly UserTeamOrganizationContext _userTeamOrganizationContext;
        public UserRepository(UserTeamOrganizationContext UserTeamOrganizationContext)
        {
            _userTeamOrganizationContext = UserTeamOrganizationContext;
        }

        public async Task<string> RegisterUser(UserBaseWithTeamIDDTO userBase, Guid userID)
        {
            Team? team = await _userTeamOrganizationContext.Teams.FindAsync(userBase.TeamId);

            if (team == null)
            {
                throw new NotFoundException("Could not find team for user");
            }

            Person person = new()
            {
                PersonId = Guid.NewGuid(),
                FirstName = userBase.FirstName,
                LastName = userBase.LastName,
                Email = userBase.Email,
                Password = userBase.Password,
                ImageUrl = null,
                DeletedAt = null,
            };

            await _userTeamOrganizationContext.Persons.AddAsync(person);

            User user = new()
            {
                UserId = userID,
                PersonId = person.PersonId,
                DeletedAt = null,

            };
            await _userTeamOrganizationContext.Users.AddAsync(user);

            UserTeam userTeam = new UserTeam()
            {
                UserId = user.UserId,
                TeamId = userBase.TeamId,
                Role = Role.User,
                DateJoined = DateTime.Now,
                DeletedAt = null,
            };
            await _userTeamOrganizationContext.UserTeams.AddAsync(userTeam);

            await _userTeamOrganizationContext.SaveChangesAsync();
            return person.Email;
        }

        /* This method is not async because I keep getting a reference loop error when returning the object 
         * I feel like I spent too much time trying to fix this, so I'm leaving it open.
         */
        public User? GetUser(Guid userId)
        {
            return _userTeamOrganizationContext.Users
                    .Where(n => n.UserId == userId && n.DeletedAt == null)
                    .Include(c => c.Person)
                    .FirstOrDefault() ?? throw new NotFoundException("Could not find user.");
        }

        public Guid GetUserId(string email)
        {
            User? user = _userTeamOrganizationContext.Users
                .Where(n => n.Person.Email == email && n.DeletedAt == null)
                .Include(c => c.Person)
                .FirstOrDefault() ?? throw new NotFoundException("Could not find user.");

            return user.UserId;
            
        }

        public async Task UpdateUser(UserDTO userDTO)
        {

            Guid? personId = await _userTeamOrganizationContext.Users
                .Where(u => u.UserId == userDTO.UserId && u.Person.DeletedAt == null)
                .Select(p => p.PersonId)
                .SingleOrDefaultAsync();

            if (personId.ToString() == "00000000-0000-0000-0000-000000000000")
            {
                throw new NotFoundException("Could not find user to update");
            }

            userDTO.PersonId = (Guid) personId;

            Person? person = await _userTeamOrganizationContext.Persons
                .Where(p => p.PersonId == personId)
                .SingleOrDefaultAsync();
            _userTeamOrganizationContext.Entry(person).CurrentValues.SetValues(userDTO);

            await _userTeamOrganizationContext.SaveChangesAsync();
        }

        public async Task DeleteUser(Guid userId)
        {
            Model.Entity.User? user = await _userTeamOrganizationContext.Users
                .Where(u => u.UserId == userId && u.DeletedAt == null)
                .FirstOrDefaultAsync() ?? throw new NotFoundException("Could not find user to delete");
            user.DeletedAt = DateTime.Now;


            Person? person = await _userTeamOrganizationContext.Persons
                .Where(p => p.PersonId == user.PersonId && p.DeletedAt == null)
                .FirstOrDefaultAsync() ?? throw new NotFoundException("Could not find person to delete");
            person.DeletedAt = DateTime.Now;

            UserTeam? userTeam = await _userTeamOrganizationContext.UserTeams
                .Where(ut => ut.UserId == userId && ut.DeletedAt == null)
                .FirstOrDefaultAsync() ?? throw new NotFoundException("Could not find userTeam to delete");
            userTeam.DeletedAt = DateTime.Now;
            await _userTeamOrganizationContext.SaveChangesAsync();
        }

        public async Task<Guid> LoginUser(string email, string password)
        {
            using (var hashAlgorithm = SHA512.Create())
            {
                var byteValue = Encoding.UTF8.GetBytes(password);
                var byteHash = hashAlgorithm.ComputeHash(byteValue);
                password = Convert.ToBase64String(byteHash);
            }

            Person? person = await _userTeamOrganizationContext.Persons
                .Where(p => p.Email == email && p.Password == password && p.DeletedAt == null)
                .FirstOrDefaultAsync() ?? throw new NotFoundException("Credentials were not correct");

            return person.PersonId;
        }

        public async Task<Dictionary<string, string>> GetRolesAndAccessLevel(Guid personId)
        {
            Dictionary<string, string> rolesAndAccessLevel = new Dictionary<string, string>();

            User? user = await _userTeamOrganizationContext.Users
                .Where(u => u.PersonId == personId && u.DeletedAt == null)
                .FirstOrDefaultAsync();

            Admin? admin = await _userTeamOrganizationContext.Admins
                .Where(a => a.PersonId == personId && a.DeletedAt == null)
                .FirstOrDefaultAsync();

            if (user != null)
            {
                List<UserTeam> userTeams = await _userTeamOrganizationContext.UserTeams
                    .Where(_ut => _ut.UserId == user.UserId)
                    .ToListAsync();

                foreach (UserTeam userTeam in userTeams)
                {
                    rolesAndAccessLevel[userTeam.TeamId.ToString()] = userTeam.Role.ToString();
                }
            }

            if (admin != null)
            {
                rolesAndAccessLevel["Accesslevel"] = admin.accessLevel.ToString();
            }

            return rolesAndAccessLevel;
        }

        public async Task<List<UserDTO>> GetUsers(Guid teamId)
        {
            List<UserDTO> users = await _userTeamOrganizationContext.UserTeams
                .Where(ut => ut.TeamId == teamId && ut.DeletedAt == null)
                .Select(ut => new UserDTO
                {
                    UserId = ut.UserId,
                    FirstName = ut.User.Person.FirstName,
                    LastName = ut.User.Person.LastName,
                    Email = ut.User.Person.Email,
                })
                .ToListAsync();

            return users;
        }

        //Delete user from team
        public async Task DeleteUserFromTeam(Guid userId, Guid teamId)
        {
            UserTeam? userTeam = await _userTeamOrganizationContext.UserTeams
                .Where(ut => ut.UserId == userId && ut.TeamId == teamId && ut.DeletedAt == null)
                .FirstOrDefaultAsync() ?? throw new NotFoundException("Could not find userTeam to delete");

            userTeam.DeletedAt = DateTime.Now;
            await _userTeamOrganizationContext.SaveChangesAsync();
        }


        public void LogoutUser(Guid userId)
        {
            throw new NotImplementedException();
        }

    }
}

