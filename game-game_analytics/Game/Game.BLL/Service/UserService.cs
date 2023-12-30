using Azure.Storage.Queues;
using Newtonsoft.Json;
using Game.BLL.Interfaces;
using Game.DAL.Interfaces;
using Game.Models.DTO;
using Game.Models.Entity;

namespace Game.BLL
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }


        public async Task RegisterUser(UserBaseWithIDAndTeamIDDTO userBase)
        {
            User user = new()
            {
                UserID = userBase.UserId,
                Firstname = userBase.FirstName,
                Lastname = userBase.LastName,
                Email = userBase.Email,
                DeletedAt = null
            };
            UserTeam userTeam = new()
            {
                UserID = userBase.UserId,
                TeamID = userBase.TeamId,
                DateJoined = DateTime.Now,
                DeletedAt = null
            };

            await _userRepository.RegisterUser(user, userTeam);
        }

        public async Task UpdateUser(UserDTO userDTO)
        {
            User user = new()
            {
                UserID = userDTO.UserId,
                Firstname = userDTO.FirstName,
                Lastname = userDTO.LastName,
                Email = userDTO.Email,
                ImageUrl = userDTO.ImageUrl
            };

            await _userRepository.UpdateUser(user);
        }

        public async Task DeleteUserFromTeam(UserTeamDTO userTeam)
        {
            await _userRepository.DeleteUserFromTeam(userTeam.UserId, userTeam.TeamId);
        }


        public async Task DeleteUser(Guid userId)
        {
            await _userRepository.DeleteUser(userId);
        }
    }
}

