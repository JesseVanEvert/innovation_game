using UserTeamOrg.BLL.Interfaces;
using UserTeamOrg.Model.DTO;
using UserTeamOrg.DAL.IRepository;
using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserTeamOrg.Model.Entity;
using UserTeamOrg.Exceptions;
using Azure.Storage.Queues;
using UserTeamOrg.BLL.Service;
using Newtonsoft.Json;

namespace UserTeamOrg.BLL
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly QueueClient queue;
        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
            queue = QueueService.GetQueueClient();
        }


        public async Task<string> RegisterUser(UserBaseWithTeamIDDTO userBase)
        {
            Guid userID = Guid.NewGuid();
            Guid teamID = userBase.TeamId;

            await _userRepository.RegisterUser(userBase, userID);
            
            UserBaseWithIDAndTeamIDDTO personDTO = new()
            {                
                TeamId = teamID,
                UserId = userID,
                FirstName = userBase.FirstName,
                LastName = userBase.LastName,
                Email = userBase.Email,
            };

            /*
                if there were no exceptions when executing the RegisterUser function, a message with the object is send via the queue
                to be put in the replicationdatabase in the Game repository
            */
            KeyValuePair<string, string> queueMessage = new("CreateUser", JsonConvert.SerializeObject(personDTO));
            await QueueService.AddMessageAsJsonAsync(queueMessage, queue);

            return userBase.Email;
        }

        //Get users belonging to a team
        public async Task<List<UserDTO>> GetUsers(Guid teamId)
        {
            List<UserDTO> users = await _userRepository.GetUsers(teamId);
            return users;
        }

        public UserDTO GetUser(Guid userId)
        {          
            User? user = _userRepository.GetUser(userId);
            UserDTO dto = new UserDTO()
            {
                Email = user.Person.Email,
                FirstName = user.Person.FirstName,
                LastName = user.Person.LastName,
                ImageUrl = user.Person.ImageUrl
            };
            return dto;
        }

        public Guid GetUserId(string email)
        {
            Guid userId =  _userRepository.GetUserId(email);           
            return userId;
        }

        public async Task UpdateUser(UserDTO userDTO)
        {
            await _userRepository.UpdateUser(userDTO);

            /*
                if there were no exceptions when executing the UpdateUser function, a message with the object is send via the queue
                to be put in the replicationdatabase in the Game repository
            */
            KeyValuePair<string, string> queueMessage = new("UpdateUser", JsonConvert.SerializeObject(userDTO));
            await QueueService.AddMessageAsJsonAsync(queueMessage, queue);
        }

        public async Task DeleteUser(Guid userId)
        {
            await _userRepository.DeleteUser(userId);

            /*
                if there were no exceptions when executing the DeleteUser function, a message with the object is send via the queue
                to be put in the replicationdatabase in the Game repository
            */
            KeyValuePair<string, string> queueMessage = new("DeleteUser", JsonConvert.SerializeObject(userId));
            await QueueService.AddMessageAsJsonAsync(queueMessage, queue);
        }

        //Delete user from team
        public async Task DeleteUserFromTeam(UserTeamDTO userTeam)
        {
            await _userRepository.DeleteUserFromTeam(userTeam.UserId, userTeam.TeamId);

            /*
                if there were no exceptions when executing the DeleteUserFromTeam function, a message with the object is send via the queue
                to be put in the replicationdatabase in the Game repository
            */
            KeyValuePair<string, string> queueMessage = new("DeleteUserFromTeam", JsonConvert.SerializeObject(userTeam));
            await QueueService.AddMessageAsJsonAsync(queueMessage, queue);
        }

        public async Task<Guid> LoginUser(string email, string password)
        {
            return await _userRepository.LoginUser(email, password);
        }

        public async Task<Dictionary<string, string>> GetRolesAndAccessLevel(Guid personId)
        {
            return await _userRepository.GetRolesAndAccessLevel(personId);
        }


        public void LogoutUser(Guid userId)
        {
            throw new NotImplementedException();
        }
    }
}

