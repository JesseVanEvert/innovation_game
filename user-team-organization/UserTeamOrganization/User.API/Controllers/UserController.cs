using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Abstractions;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Enums;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Resolvers;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using UserTeamOrg.Model.DTO;
using UserTeamOrg.BLL.Interfaces;
using UserTeamOrg.Exceptions;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Azure.Functions.Worker;
using UserTeamOrg.Model.Entity;
using System.Security.Claims;
using System.Collections.Generic;
using System.Net.Http.Headers;
using Azure.Core;
using Microsoft.IdentityModel.Tokens;

namespace User.API.Controllers
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

    public class UserController
    {
        private readonly ILogger<UserController> _logger;
        private IUserService userService;
        private ITokenService _tokenService;

        public UserController(ILogger<UserController> log, IUserService userService, ITokenService tokenService)
        {
            _logger = log;
            this.userService = userService;
            _tokenService = tokenService;
        }    

        // Register User endpoint
        [FunctionName("RegisterUser")]
        [Auth]
        [OpenApiOperation(operationId: "RegisterUser", tags: new[] { "User" }, Summary = "Register a new user", Description = "This endpoint will register a new user to a team. A TeamID and OrganizationID is required to create a new user.")]
        [OpenApiRequestBody("application/json", typeof(UserBaseWithTeamIDDTO), Description = "User data.", Example = typeof(UserBaseWithTeamIDDTOExampleGenerator))]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(string), Description = "The Created response with the newly registered user.")]
        public async Task<IActionResult> RegisterUser(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "User")] HttpRequest req)
        {
            // Authentication and authorization - Everyone can use this endpoint

            _logger.LogInformation("Registering a new user.");

            // Parse input
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            UserBaseWithTeamIDDTO userBase = new();
            try
            {
                userBase = JsonConvert.DeserializeObject<UserBaseWithTeamIDDTO>(requestBody);
            }
            catch (JsonSerializationException e)
            {
                return new BadRequestObjectResult(e.Message);
            }

            //Check if any of the fields of userBase are empty
            if (userBase.Email.IsNullOrEmpty() || userBase.FirstName.IsNullOrEmpty() || userBase.LastName.IsNullOrEmpty() || userBase.Password.IsNullOrEmpty() || userBase.TeamId == null)
            {
                return new BadRequestObjectResult("One or more fields are empty");
            }

            string email;
            try
            {
                email = await userService.RegisterUser(userBase);
                
            }            
            catch (NotFoundException e)
            {
                return new NotFoundObjectResult(e.Message);
            }
            catch (Exception ex)
            {
                _logger.LogInformation($"Unable to register user, exception message: {ex.Message}");
                return new BadRequestObjectResult(ex.Message);
            }

            return new OkObjectResult("");
        }
        
        //Delete user from team
        [FunctionName("DeleteUserFromTeam")]
        [Auth]
        [OpenApiOperation(operationId: "DeleteUserFromTeam", tags: new[] { "User" }, Summary = "Delete user from team", Description = "This endpoint will delete an existing user from a team. It requires a userID and a teamID.")]
        [OpenApiRequestBody("application/json", typeof(UserTeamDTO), Description = "User data.")]
        [OpenApiResponseWithoutBody(HttpStatusCode.OK, Description = "User was succesfully deleted from team.")]
        public async Task<IActionResult> DeleteUserFromTeam(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "User/DeleteUserFromTeam")] HttpRequest req)
        {
            // Authentication
            Dictionary<string, string> auth = await _tokenService.GetRequestAuth(req);
            if (auth == null)
            {
                return new ObjectResult("No authentication was found or authentication was not a valid token") { StatusCode = 403 };
            }

            // Authorization - Everyone can use this endpoint

            _logger.LogInformation("Deleting user from team.");
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            UserTeamDTO userTeamDTO = new();
            try
            {
                userTeamDTO = JsonConvert.DeserializeObject<UserTeamDTO>(requestBody);
            }
            catch (JsonSerializationException e)
            {
                return new BadRequestObjectResult(e.Message);
            }
            try
            {
                await userService.DeleteUserFromTeam(userTeamDTO);
            }
            catch (NotFoundException e)
            {
                return new NotFoundObjectResult(e.Message);
            }
            catch (Exception ex)
            {
                _logger.LogInformation($"Unable to delete user from team, exception message: {ex.Message}");
                return new BadRequestObjectResult(ex.Message);
            }
            return new OkObjectResult("");
        }

        //Get users belonging to a team
        [FunctionName("GetUsers")]
        [Auth]
        [OpenApiOperation(operationId: "GetUsers", tags: new[] { "User" }, Summary = "Get users belonging to a team", Description = "This endpoint will retrieve all users belonging to a team. It requires a teamID.")]
        [OpenApiSecurity("function_key", SecuritySchemeType.ApiKey, Name = "code", In = OpenApiSecurityLocationType.Query)]
        [OpenApiParameter(name: "teamId", In = ParameterLocation.Query, Required = true, Type = typeof(Guid), Description = "The Id of the team.")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(List<UserDTO>), Description = "The OK response with the retrieved users.", Example = typeof(UserDTOExampleGenerator))]
        public async Task<IActionResult> GetUsers(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "User/GetUsers")] HttpRequest req)
        {
            // Authentication
            Dictionary<string, string> auth = await _tokenService.GetRequestAuth(req);
            if (auth == null)
            {
                return new ObjectResult("No authentication was found or authentication was not a valid token") { StatusCode = 403 };
            }

            // Authorization - Everyone can use this endpoint

            _logger.LogInformation("Retrieving users.");
            Guid teamId;
            List<UserDTO> retrievedUsers = new();
            try
            {
                teamId = Guid.Parse(req.Query["teamId"]);
            }
            catch (JsonSerializationException e)
            {
                return new BadRequestObjectResult(e.Message);
            }
            try
            {
                retrievedUsers = await userService.GetUsers(teamId);
            }
            catch (NotFoundException ex)
            {
                return new NotFoundObjectResult(ex.Message);
            }
            catch (Exception ex)
            {
                return new BadRequestObjectResult(ex.Message);
            }
            return new OkObjectResult(retrievedUsers);
        }
        [FunctionName("GetUserId")]
        [Auth]
        [OpenApiOperation(operationId: "GetUserId", tags: new[] { "User" }, Summary = "Get userId", Description = "This endpoint will retrieve existing userId. It requires an Email.")]
        [OpenApiSecurity("function_key", SecuritySchemeType.ApiKey, Name = "code", In = OpenApiSecurityLocationType.Query)]
        [OpenApiParameter(name: "email", In = ParameterLocation.Query, Required = true, Type = typeof(string), Description = "The email of the user.")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(Guid), Description = "The OK response with the retrieved userId.")]
        public async Task<IActionResult> GetUserId(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "User/GetUserId")] HttpRequest req)
        {
            // Authentication
            Dictionary<string, string> auth = await _tokenService.GetRequestAuth(req);
            if (auth == null)
            {
                return new ObjectResult("No authentication was found or authentication was not a valid token") { StatusCode = 403 };
            }

            // Authorization - Everyone can use this endpoint

            _logger.LogInformation("Retrieving userId.");
            string email;
            Guid userId;
            try
            {
                email = (req.Query["email"]).ToString();
            }
            catch (JsonSerializationException e)
            {
                return new BadRequestObjectResult(e.Message);
            }
            try
            {
                userId = userService.GetUserId(email);
            }
            catch (NotFoundException ex)
            {
                return new NotFoundObjectResult(ex.Message);
            }
            catch (Exception ex)
            {
                return new BadRequestObjectResult(ex.Message);
            }
            return new OkObjectResult(userId);
        }

        // Get User endpoint
        [FunctionName("GetUser")]
        [Auth]
        [OpenApiOperation(operationId: "GetUser", tags: new[] { "User" }, Summary = "Get user information", Description = "This endpoint will retrieve existing user information. It requires a userID.")]
        [OpenApiSecurity("function_key", SecuritySchemeType.ApiKey, Name = "code", In = OpenApiSecurityLocationType.Query)]
        [OpenApiParameter(name: "userId", In = ParameterLocation.Query, Required = true, Type = typeof(Guid), Description = "The Id of the user.")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(UserDTO), Description = "The OK response with the retrieved user.", Example = typeof(UserDTOExampleGenerator))]
        public async Task<IActionResult> GetUser(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "User")] HttpRequest req)
        {
            // Authentication
            Dictionary<string, string> auth = await _tokenService.GetRequestAuth(req);
            if (auth == null)
            {
                return new ObjectResult("No authentication was found or authentication was not a valid token") { StatusCode = 403 };
            }

            // Authorization - Everyone can use this endpoint

            _logger.LogInformation("Retrieving user.");
            Guid userId;
            UserDTO retrievedUser = new();
            try
            {
                userId = Guid.Parse(req.Query["userId"]);
            }
            catch (JsonSerializationException e)
            {
                return new BadRequestObjectResult(e.Message);
            }
            try
            {                
                 retrievedUser = userService.GetUser(userId);                
            }
            catch (NotFoundException ex)
            {                
                return new NotFoundObjectResult(ex.Message);
            }
            catch (Exception ex)
            {
                return new BadRequestObjectResult(ex.Message);
            }
            return new OkObjectResult(retrievedUser);
        }

        // Update User endpoint
        [FunctionName("UpdateUser")]
        [Auth]
        [OpenApiOperation(operationId: "UpdateUser", tags: new[] { "User" }, Summary = "Update user information", Description = "This endpoint will update an existing user's information. It requires a userID.")]
        [OpenApiSecurity("function_key", SecuritySchemeType.ApiKey, Name = "code", In = OpenApiSecurityLocationType.Query)]
        [OpenApiRequestBody("application/json", typeof(UserDTO), Description = "User data.", Example = typeof(UserDTOExampleGenerator))]
        [OpenApiResponseWithoutBody(HttpStatusCode.OK, Description = "User information was succesfully updated.")]
        public async Task<IActionResult> UpdateUser(
            [HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "User")] HttpRequest req)
        {
            // Authentication
            Dictionary<string, string> auth = await _tokenService.GetRequestAuth(req);
            if (auth == null)
            {
                return new ObjectResult("No authentication was found or authentication was not a valid token") { StatusCode = 403 };
            }

            // Authorization - Everyone can use this endpoint

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();           
            _logger.LogInformation("Updating user information.");
            UserDTO userBase = new();
            try
            {
                userBase = JsonConvert.DeserializeObject<UserDTO>(requestBody);               
            }
            catch (JsonSerializationException e)
            {
                return new BadRequestObjectResult(e.Message);
            }
            try
            {
                await userService.UpdateUser(userBase);                
            }
            catch (NotFoundException e)
            {
                return new NotFoundObjectResult(e.Message);
            }
            catch (Exception ex)
            {
                _logger.LogInformation($"Failed to update user information, exception message: {ex.Message}");
                return new BadRequestObjectResult(ex.Message);
            }
            return new OkObjectResult($"User's information was updated");
        }

        // Delete User endpoint
        [FunctionName("DeleteUser")]
        [Auth]
        [OpenApiOperation(operationId: "DeleteUser", tags: new[] { "User" }, Summary = "Get user information", Description = "This endpoint will delete an existing user's information. It requires a userID.")]
        [OpenApiSecurity("function_key", SecuritySchemeType.ApiKey, Name = "code", In = OpenApiSecurityLocationType.Query)]
        [OpenApiParameter(name: "userId", In = ParameterLocation.Query, Required = true, Type = typeof(Guid), Description = "The Id of the user.")]
        [OpenApiResponseWithoutBody(HttpStatusCode.OK, Description = "User was succesfully deleted.")]
        public async Task<IActionResult> DeleteUser(
            [HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = "User")] HttpRequest req)
        {
            // Authentication
            Dictionary<string, string> auth = await _tokenService.GetRequestAuth(req);
            if (auth == null)
            {
                return new ObjectResult("No authentication was found or authentication was not a valid token") { StatusCode = 403 };
            }

            // Authorization - Everyone can use this endpoint

            _logger.LogInformation("Deleting user.");
            Guid userId;
            try
            {
                userId = Guid.Parse(req.Query["userId"]);                
            }
            catch (JsonSerializationException e)
            {
                return new BadRequestObjectResult(e.Message);
            }
            try
            {
                await userService.DeleteUser(userId);                
            }
            catch (NotFoundException e)
            {
                return new NotFoundObjectResult(e.Message);
            }
            catch (Exception ex)
            {
                _logger.LogInformation($"Unable to delete user, exception message: {ex.Message}");
                return new BadRequestObjectResult(ex.Message);
            }
            return new OkObjectResult($"User was succesfully deleted");
        }

        // Login User endpoint
        [FunctionName("LoginPerson")]
        [OpenApiOperation(operationId: "LoginUser", tags: new[] { "Person" }, Summary = "Login an existing person", Description = "This endpoint will login an existing person. This requires the Email and Password of a person.")]
        [OpenApiParameter(name: "email", In = ParameterLocation.Query, Required = true, Type = typeof(string), Description = "The email of the person.")]
        [OpenApiParameter(name: "password", In = ParameterLocation.Query, Required = true, Type = typeof(string), Description = "The password of the person.")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(LoginResult), Description = "The OK response with a token")]
        public async Task<IActionResult> LoginUser(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "User/Login")] HttpRequest req)
        {
            // Authentication and authorization - Everyone can use this endpoint

            _logger.LogInformation("Logging in person");

            string email = req.Query["email"];
            string password = req.Query["password"];

            try
            {
                Guid personId = await userService.LoginUser(email, password);
                Dictionary<string, string> RolesAndAccessLevel = await userService.GetRolesAndAccessLevel(personId);

                LoginResult result = await _tokenService.CreateToken(RolesAndAccessLevel);
                _logger.LogInformation($"Person {personId} logged in succesfully");
                return new OkObjectResult(result);
            }
            catch (NotFoundException ex)
            {
                _logger.LogInformation($"Person was not found, exception message: {ex.Message}");
                return new NotFoundObjectResult(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogInformation($"Unable to log person in, exception message: {ex.Message}");
                return new BadRequestObjectResult(ex.Message);
            }
        }
    }
}

