using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Game.BLL.Interfaces;
using Game.Exceptions.Exceptions;
using Game.Models.DTO;
using Game.Models.Entity;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace UserTeamOrg.QueueTrigger
{
    public class UserTeamOrgQueueTrigger
    {

        private readonly IUserService _userService;
        private readonly ITeamService _teamService;
        private readonly IOrganizationService _organizationService;
        public UserTeamOrgQueueTrigger(IUserService userService, ITeamService teamService, IOrganizationService organizationService)
        {
            _organizationService = organizationService;
            _userService = userService;
            _teamService = teamService;
        }

        [FunctionName("UserTeamOrgQueueTrigger")]
        public async Task RunAsync([QueueTrigger("userteamorganizationqueue", Connection = "AzureWebJobsStorage")] string myQueueItem, ILogger log)
        {
            KeyValuePair<string, string> myItem = JsonConvert.DeserializeObject<KeyValuePair<string, string>>(myQueueItem);

            try
            {
                //this switch uses the key of the keyvalue pair to determine what DTO or class to use to deserialize the json object
                switch (myItem.Key)
                {
                    case "CreateUser":
                        await _userService.RegisterUser(JsonConvert.DeserializeObject<UserBaseWithIDAndTeamIDDTO>(myItem.Value));
                        break;
                    case "UpdateUser":
                        await _userService.UpdateUser(JsonConvert.DeserializeObject<UserDTO>(myItem.Value));
                        break;
                    case "DeleteUser":
                        await _userService.DeleteUser(JsonConvert.DeserializeObject<Guid>(myItem.Value));
                        break;
                    case "DeleteUserFromTeam":
                        await _userService.DeleteUserFromTeam(JsonConvert.DeserializeObject<UserTeamDTO>(myItem.Value));
                        break;
                    case "CreateTeam":
                        await _teamService.CreateTeam(JsonConvert.DeserializeObject<Team>(myItem.Value));
                        break;
                    case "UpdateTeam":
                        await _teamService.UpdateTeam(JsonConvert.DeserializeObject<Team>(myItem.Value));
                        break;
                    case "DeleteTeam":
                        await _teamService.DeleteTeam(JsonConvert.DeserializeObject<Guid>(myItem.Value));
                        break;
                    case "CreateOrganization":
                        await _organizationService.CreateOrganization(JsonConvert.DeserializeObject<Organization>(myItem.Value));
                        break;
                    case "UpdateOrganization":
                        await _organizationService.UpdateOrganization(JsonConvert.DeserializeObject<Organization>(myItem.Value));
                        break;
                    case "DeleteOrganization":
                        await _organizationService.DeleteOrganization(JsonConvert.DeserializeObject<Guid>(myItem.Value));
                        break;
                }
            }
            catch (EntityNotFoundException ex)
            {
                log.LogInformation(ex.Message);
            }
            catch(DbUpdateException ex)
            {
                log.LogInformation(ex.Message);
            }            

        }
    }
}
