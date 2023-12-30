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
using UserTeamOrg.Model.Entity;
using UserTeamOrg.BLL.Interfaces;
using UserTeamOrg.Exceptions;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Enums;
using System.Collections.Generic;

namespace Organization.API.Controllers
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

    public class OrganizationController
    {
        private ILogger _logger { get; }
        private IOrganizationService _organizationService;
        private ITokenService _tokenService;

        public OrganizationController(ILogger<OrganizationController> log, IOrganizationService organizationService, ITokenService tokenService)
        {
            _logger = log;
            _organizationService = organizationService;
            _tokenService = tokenService;
        }

        [FunctionName("CreateOrganization")]
        [Auth]
        [OpenApiOperation(operationId: "CreateOrganization", tags: new[] { "Organizations" })]
        [OpenApiRequestBody("application/json", typeof(OrganizationBaseDTO), Required = true, Example = typeof(OrganizationBaseDTOExampleGenerator))]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.Created, contentType: "application/json", bodyType: typeof(string), Description = "The Created response with name and organizationId")]
        public async Task<IActionResult> CreateOrganization([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequest req)
        {
            // Authentication
            Dictionary<string, string> auth = await _tokenService.GetRequestAuth(req);
            if (auth == null)
            {
                return new ObjectResult("No authentication was found or authentication was not a valid token") { StatusCode = 403 };
            }
            
            // Authorization - AccessLevel
            /*bool personHasCorrectAccessLevel = _tokenService.CheckAccessLevel(auth, AccessLevel.SuperAdmin);
            if (!personHasCorrectAccessLevel)
            {
                return new UnauthorizedResult();
            }*/

            _logger.LogInformation("Creating a new organization");

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            OrganizationBaseDTO body;

            try
            {
                body = JsonConvert.DeserializeObject<OrganizationBaseDTO>(requestBody);
            }
            catch (JsonSerializationException ex)
            {
                _logger.LogInformation($"Json deserialization failed, exception message: {ex.Message}");
                return new BadRequestObjectResult(ex.Message);
            }
            try
            {
                UserTeamOrg.Model.Entity.Organization organization = await _organizationService.CreateOrganization(body);
                _logger.LogInformation($"Organization {organization.OrganizationID} succesfully created");
                return new OkObjectResult(organization.OrganizationID);
                //return new CreatedAtActionResult(nameof(GetOrganization), "OrganizationController", new { id = organization.OrganizationID }, organization);

            }
            catch (Exception ex)
            {
                _logger.LogInformation($"Unable to create organization, exception message: {ex.Message}");
                return new BadRequestObjectResult(ex.Message);
            }
        }

        [FunctionName("GetOrganization")]
        [Auth]
        [OpenApiOperation(operationId: "GetOrganization", tags: new[] { "Organizations" })]
        [OpenApiParameter(name: "Id", In = ParameterLocation.Query, Required = true, Type = typeof(string), Description = "The Id of the organization")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(OrganizationBaseWithIdDTO), Description = "The OK response", Example = typeof(OrganizationBaseWithIdDTOExampleGenerator))]
        public async Task<IActionResult> GetOrganization([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = null)] HttpRequest req)
        {
            // Authentication
            Dictionary<string, string> auth = await _tokenService.GetRequestAuth(req);
            if (auth == null)
            {
                return new ObjectResult("No authentication was found or authentication was not a valid token") { StatusCode = 403 };
            }

            // Authorization - Everyone is allowed to use this endpoint

            _logger.LogInformation("Getting organization");

            try
            {
                Guid OrganizationId = Guid.Parse(req.Query["Id"]);
                UserTeamOrg.Model.Entity.Organization organization = await _organizationService.GetOrganization(OrganizationId);
                _logger.LogInformation($"Got Organization {OrganizationId} succesfully");

                return new OkObjectResult(organization);
            }
            catch (NotFoundException ex)
            {
                _logger.LogInformation($"Organization was not found, exception message: {ex.Message}");
                return new NotFoundObjectResult(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogInformation($"Unable to get organization, exception message: {ex.Message}");
                return new BadRequestObjectResult(ex.Message);
            }
        }

        [FunctionName("UpdateOrganization")]
        [Auth]
        [OpenApiOperation(operationId: "UpdateOrganization", tags: new[] { "Organizations" })]
        [OpenApiRequestBody("application/json", typeof(OrganizationBaseWithIdDTO), Required = true, Example = typeof(OrganizationBaseWithIdDTOExampleGenerator))]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(string), Description = "The OK response")]
        public async Task<IActionResult> UpdateOrganization([HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = null)] HttpRequest req)
        {
            // Authentication
            Dictionary<string, string> auth = await _tokenService.GetRequestAuth(req);
            if (auth == null)
            {
                return new ObjectResult("No authentication was found or authentication was not a valid token") { StatusCode = 403 };
            }

            // Authorization - AccessLevel
            bool personHasCorrectAccessLevel = _tokenService.CheckAccessLevel(auth, AccessLevel.SuperAdmin);
            if (!personHasCorrectAccessLevel)
            {
                return new UnauthorizedResult();

            }

            _logger.LogInformation("Updating organization");

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            OrganizationBaseWithIdDTO body;

            try
            {
                body = JsonConvert.DeserializeObject<OrganizationBaseWithIdDTO>(requestBody);
            }
            catch (JsonSerializationException ex)
            {
                _logger.LogInformation($"Json deserialization failed, exception message: {ex.Message}");
                return new BadRequestObjectResult(ex.Message);
            }
            try
            {
                await _organizationService.UpdateOrganization(body);
                _logger.LogInformation($"Got organization {body.OrganizationId} succesfully");

                return new OkObjectResult($"The organization with id: '{body.OrganizationId}' was updated successfully.");
            }
            catch (NotFoundException ex)
            {
                _logger.LogInformation($"Organization was not found, exception message: {ex.Message}");
                return new NotFoundObjectResult(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogInformation($"Unable to update organization, exception message: {ex.Message}");
                return new BadRequestObjectResult(ex.Message);
            }
        }

        [FunctionName("DeleteOrganization")]
        [Auth]
        [OpenApiOperation(operationId: "DeleteOrganization", tags: new[] { "Organizations" })]
        [OpenApiParameter(name: "Id", In = ParameterLocation.Query, Required = true, Type = typeof(string), Description = "The Id of the organization")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(string), Description = "The OK response")]
        public async Task<IActionResult> DeleteOrganization([HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = null)] HttpRequest req)
        {
            // Authentication
            Dictionary<string, string> auth = await _tokenService.GetRequestAuth(req);
            if (auth == null)
            {
                return new ObjectResult("No authentication was found or authentication was not a valid token") { StatusCode = 403 };
            }

            // Authorization - AccessLevel
            bool personHasCorrectAccessLevel = _tokenService.CheckAccessLevel(auth, AccessLevel.SuperAdmin);
            if (!personHasCorrectAccessLevel)
            {
                return new UnauthorizedResult();

            }

            _logger.LogInformation("Deleting organization");

            try
            {
                Guid OrganizationId = Guid.Parse(req.Query["Id"]);
                await _organizationService.DeleteOrganization(OrganizationId);
                _logger.LogInformation($"Organization {OrganizationId} was succesfully deleted");

                return new OkObjectResult("Organization was succesfully deleted");
            }
            catch (NotFoundException ex)
            {
                _logger.LogInformation($"Organization was not found, exception message: {ex.Message}");
                return new NotFoundObjectResult(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogInformation($"Unable to delete organization, exception message: {ex.Message}");
                return new BadRequestObjectResult(ex.Message);
            }
        }

        //Return all organizations
        [FunctionName("GetOrganizations")]
        [Auth]
        [OpenApiOperation(operationId: "GetOrganizations", tags: new[] { "Organizations" })]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(IEnumerable<UserTeamOrg.Model.Entity.Organization>), Description = "The OK response")]
        public async Task<IActionResult> GetOrganizations([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = null)] HttpRequest req)
        {
            // Authentication
            Dictionary<string, string> auth = await _tokenService.GetRequestAuth(req);
            if (auth == null)
            {
                return new ObjectResult("No authentication was found or authentication was not a valid token") { StatusCode = 403 };
            }

            // Authorization - Everyone is allowed to use this endpoint

            _logger.LogInformation("Getting all organizations");

            try
            {
                IEnumerable<UserTeamOrg.Model.Entity.Organization> organizations = await _organizationService.GetOrganizations();
                _logger.LogInformation($"Got all organizations succesfully");

                return new OkObjectResult(organizations);
            }
            catch (Exception ex)
            {
                _logger.LogInformation($"Unable to get all organizations, exception message: {ex.Message}");
                return new BadRequestObjectResult(ex.Message);
            }
        }
    }
}
