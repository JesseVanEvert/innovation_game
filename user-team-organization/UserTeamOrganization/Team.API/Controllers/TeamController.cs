using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using UserTeamOrg.Model.DTO;
using UserTeamOrg.BLL.Interfaces;
using UserTeamOrg.BLL;
using System.Security.Claims;
using System.Collections.Generic;
using UserTeamOrg.Model.Entity;
using UserTeamOrg.Exceptions;
using System.Web.Http;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Enums;

namespace Team.API.Controllers
{
    public class AuthAttribute : OpenApiSecurityAttribute
    {
        public AuthAttribute() : base("Auth", SecuritySchemeType.Http)
        {
            Description = "JWT for authorization and authentication";
            In = OpenApiSecurityLocationType.Header;
            Scheme = OpenApiSecuritySchemeType.Bearer;
            BearerFormat = "JWT";
        }
    }

    public class TeamController
    {
        private ILogger _logger { get; }
        private ITeamService _teamService;
        private IMailService _mailService;
        private ITokenService _tokenService;

        public TeamController(ILogger<TeamController> log, ITeamService teamService, ITokenService tokenService, IMailService mailService)
        { 
            _logger = log;
            _teamService = teamService;
            _tokenService = tokenService;
            _mailService = mailService;
        }
        
        //Get all teams belonging to an organization
        [FunctionName("GetTeams")]
        [Auth]
        [OpenApiOperation(operationId: "GetTeams", tags: new[] { "Teams" })]
        [OpenApiParameter(name: "OrganizationId", In = ParameterLocation.Query, Required = true, Type = typeof(string), Description = "The Id of the organization")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(IEnumerable<UserTeamOrg.Model.Entity.Team>), Description = "The OK response")]
        public async Task<IActionResult> GetTeams([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = null)] HttpRequest req)
        {
            // Authentication
            Dictionary<string, string> auth = await _tokenService.GetRequestAuth(req);
            if (auth == null)
            {
                return new ObjectResult("No authentication was found or authentication was not a valid token") { StatusCode = 403 };
            }

            // Authorization - Everyone can use this endpoint

            _logger.LogInformation("Getting teams");

            try
            {
                Guid organizationId = Guid.Parse(req.Query["OrganizationId"]);
                IEnumerable<UserTeamOrg.Model.Entity.Team> teams = await _teamService.GetTeams(organizationId);
                _logger.LogInformation($"Got teams from organization {organizationId} succesfully");

                return new OkObjectResult(teams);
            }
            catch (NotFoundException ex)
            {
                _logger.LogInformation($"Organization was not found, exception message: {ex.Message}");
                return new NotFoundObjectResult(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogInformation($"Unable to get teams, exception message: {ex.Message}");
                return new BadRequestObjectResult(ex.Message);
            }
        }
        [FunctionName("CreateTeam")]
        [Auth]
        [OpenApiOperation(operationId: "CreateTeam", tags: new[] { "Teams" })]
        [OpenApiRequestBody("application/json", typeof(TeamBaseDTO), Required = true, Example = typeof(TeamBaseDTOExampleGenerator))]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.Created, contentType: "application/json", bodyType: typeof(string), Description = "The Created response with name and organizationId")]
        public async Task<IActionResult> CreateTeam([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequest req)
        {
            // Authentication
            Dictionary<string, string> auth = await _tokenService.GetRequestAuth(req);
            if (auth == null)
            {
                return new ObjectResult("No authentication was found or authentication was not a valid token") { StatusCode = 403 };
            }
            /*
            // Authorization - AccessLevel
            bool personHasCorrectAccessLevel = _tokenService.CheckAccessLevel(auth, AccessLevel.Admin);
            if (!personHasCorrectAccessLevel)
            {
                return new UnauthorizedResult();

            }*/

            _logger.LogInformation("Creating a new team");

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            TeamBaseDTO body;

            try
            {
                body = JsonConvert.DeserializeObject<TeamBaseDTO>(requestBody);
            }
            catch (JsonSerializationException ex)
            {
                _logger.LogInformation($"Json deserialization failed, exception message: {ex.Message}");
                return new BadRequestObjectResult(ex.Message);
            }
            try
            {
                UserTeamOrg.Model.Entity.Team team = await _teamService.CreateTeam(body);
                _logger.LogInformation($"Team {team.TeamID} succesfully created");
                return new OkObjectResult(team.TeamID);
            }
            catch (Exception ex)
            {
                _logger.LogInformation($"Unable to create team, exception message: {ex.Message}");
                return new BadRequestObjectResult(ex.Message);
            }
        }

        [FunctionName("GetTeam")]
        [Auth]
        [OpenApiOperation(operationId: "GetTeam", tags: new[] { "Teams" })]
        [OpenApiParameter(name: "Id", In = ParameterLocation.Query, Required = true, Type = typeof(string), Description = "The Id of the Team")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(TeamBaseWithIdDTO), Description = "The OK response", Example = typeof(TeamBaseWithIdDTOExampleGenerator))]
        public async Task<IActionResult> GetTeam([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = null)] HttpRequest req)
        {
            // Authentication
            Dictionary<string, string> auth = await _tokenService.GetRequestAuth(req);
            if (auth == null)
            {
                return new ObjectResult("No authentication was found or authentication was not a valid token") { StatusCode = 403 };
            }

            // Authorization - Everyone can use this endpoint

            _logger.LogInformation("Getting team");

            try
            {
                Guid TeamId = Guid.Parse(req.Query["Id"]);
                UserTeamOrg.Model.Entity.Team team = await _teamService.GetTeam(TeamId);
                _logger.LogInformation($"Got team {TeamId} succesfully");

                return new OkObjectResult(team);
            }
            catch (NotFoundException ex)
            {
                _logger.LogInformation($"Team was not found, exception message: {ex.Message}");
                return new NotFoundObjectResult(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogInformation($"Unable to get team, exception message: {ex.Message}");
                return new BadRequestObjectResult(ex.Message);
            }
        }

        [FunctionName("UpdateTeam")]
        [Auth]
        [OpenApiOperation(operationId: "UpdateTeam", tags: new[] { "Teams" })]
        [OpenApiRequestBody("application/json", typeof(TeamBaseWithIdDTO), Required = true, Example = typeof(TeamBaseWithIdDTOExampleGenerator))]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(string), Description = "The OK response")]
        public async Task<IActionResult> UpdateTeam([HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = null)] HttpRequest req)
        {
            // Authentication
            Dictionary<string, string> auth = await _tokenService.GetRequestAuth(req);
            if (auth == null)
            {
                return new ObjectResult("No authentication was found or authentication was not a valid token") { StatusCode = 403 };
            }

            // Authorization - AccessLevel
            /*
            bool personHasCorrectAccessLevel = _tokenService.CheckAccessLevel(auth, AccessLevel.Admin);
            if (!personHasCorrectAccessLevel)
            {
                return new UnauthorizedResult();

            }
            */

            _logger.LogInformation("Updating organization");

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            TeamBaseWithIdDTO body;

            try
            {
                body = JsonConvert.DeserializeObject<TeamBaseWithIdDTO>(requestBody);
            }
            catch (JsonSerializationException ex)
            {
                _logger.LogInformation($"Json deserialization failed, exception message: {ex.Message}");
                return new BadRequestObjectResult(ex.Message);
            }
            try
            {
                await _teamService.UpdateTeam(body);
                _logger.LogInformation($"Got team {body.TeamId} succesfully");

                return new OkObjectResult($"The Team with id: '{body.TeamId}' was updated successfully.");
            }
            catch (NotFoundException ex)
            {
                _logger.LogInformation($"Team was not found, exception message: {ex.Message}");
                return new NotFoundObjectResult(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogInformation($"Unable to update team, exception message: {ex.Message}");
                return new BadRequestObjectResult(ex.Message);
            }
        }

        [FunctionName("GetAllTeams")]
        [Auth]
        [OpenApiOperation(operationId: "GetAllTeams", tags: new[] {"Teams"})]
        [OpenApiParameter(name: "Id", In = ParameterLocation.Query, Required = true, Type = typeof(string), Description = "The Id of the user")]
        [OpenApiResponseWithBody(statusCode:HttpStatusCode.OK, contentType:"application/json", bodyType: typeof(string), Description = "The OK response")]
        public async Task<IActionResult> GetAllTeams([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = null)] HttpRequest req)
        {
            // Authentication
            Dictionary<string, string> auth = await _tokenService.GetRequestAuth(req);
            if (auth == null)
            {
                return new ObjectResult("No authentication was found or authentication was not a valid token") { StatusCode = 403 };
            }

            try
            {
                Guid TeamId = Guid.Parse(req.Query["Id"]);
                HashSet<TeamBaseWithIdDTO> teamsDTO = _teamService.GetAllTeamsOfUser(TeamId);
                _logger.LogInformation($"Got team {TeamId} succesfully");

                return new OkObjectResult(teamsDTO);
            }
            catch (NotFoundException ex)
            {
                _logger.LogInformation($"Team was not found, exception message: {ex.Message}");
                return new NotFoundObjectResult(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogInformation($"Unable to get team, exception message: {ex.Message}");
                return new BadRequestObjectResult(ex.Message);
            }
        }

        [FunctionName("DeleteTeam")]
        [Auth]
        [OpenApiOperation(operationId: "DeleteTeam", tags: new[] { "Teams" })]
        [OpenApiParameter(name: "Id", In = ParameterLocation.Query, Required = true, Type = typeof(string), Description = "The Id of the Team")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(string), Description = "The OK response")]
        public async Task<IActionResult> DeleteTeam([HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = null)] HttpRequest req)
        {
            // Authentication
            Dictionary<string, string> auth = await _tokenService.GetRequestAuth(req);
            if (auth == null)
            {
                return new ObjectResult("No authentication was found or authentication was not a valid token") { StatusCode = 403 };
            }

            // Authorization - AccessLevel
            /*
            bool personHasCorrectAccessLevel = _tokenService.CheckAccessLevel(auth, AccessLevel.Admin);
            if (!personHasCorrectAccessLevel)
            {
                return new UnauthorizedResult();

            }
            */

            _logger.LogInformation("Deleting team");

            try
            {
                Guid TeamId = Guid.Parse(req.Query["Id"]);
                await _teamService.DeleteTeam(TeamId);
                _logger.LogInformation($"Team {TeamId} was succesfully deleted");

                return new OkObjectResult($"The team with TeamId: {TeamId} has been deleted");
            }
            catch (NotFoundException ex)
            {
                _logger.LogInformation($"Team was not found, exception message: {ex.Message}");
                return new NotFoundObjectResult(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogInformation($"Unable to delete team, exception message: {ex.Message}");
                return new BadRequestObjectResult(ex.Message);
            }
        }

        [FunctionName("SendEmail")]
        [OpenApiOperation(operationId: "SendEmail", tags: new[] { "Teams" })]
        [OpenApiRequestBody("application/json", typeof(EmailDTO), Required = true, Example = typeof(EmailDTOExampleGenerator))]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(string), Description = "The OK response")]
        public async Task<IActionResult> SendEmail([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequest req)
        {
            // Authentication
            Dictionary<string, string> auth = await _tokenService.GetRequestAuth(req);
            if (auth == null)
            {
                return new ObjectResult("No authentication was found or authentication was not a valid token") { StatusCode = 403 };
            }

            // Authorization - Everyone can use this endpoint

            _logger.LogInformation("Sending email");

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            EmailDTO body;

            try
            {
                body = JsonConvert.DeserializeObject<EmailDTO>(requestBody);
            }
            catch (JsonSerializationException ex)
            {
                _logger.LogInformation($"Json deserialization failed, exception message: {ex.Message}");
                return new BadRequestObjectResult(ex.Message);
            }

            bool mailSent;
            try
            {
                mailSent = await _mailService.SendEmail(body);

            }
            catch (Exception ex)
            {
                _logger.LogInformation($"Unable to send mail, exception message: {ex.Message}");
                return new BadRequestObjectResult(ex.Message);
            }

            if (mailSent)
            {
                _logger.LogInformation($"Mail has been sent to {body.Email} was succesfully deleted");
                return new OkObjectResult($"Mail has been sent to {body.Email}");
            }

            _logger.LogInformation($"Unable to send mail to {body.Email}");
            return new InternalServerErrorResult();
        }
    }
}
