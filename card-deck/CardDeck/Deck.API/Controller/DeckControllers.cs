using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http;
using CardDeck.BLL.Interfaces;
using CardDeck.Exceptions.Exceptions;
using CardDeck.Model.DTO;
using CardDeck.Model.Entity;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;

namespace Deck.API.Controller
{
    public class DeckControllers
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

        private readonly ILogger<DeckControllers> _logger;
        private IDeckService _deckService;
        private ITokenService _tokenService;

        public DeckControllers(ILogger<DeckControllers> log, IDeckService deckService, ITokenService tokenService)
        {
            _logger = log;
            _deckService = deckService;
            _tokenService = tokenService;
        }


        [FunctionName("CreateDeck")]
        [Auth]
        [OpenApiOperation(operationId: "CreateDeck", tags: new[] { "Deck" })]
        [OpenApiRequestBody("application/json", typeof(DeckBaseDTO), Description = "The name of the new deck you want to make and the teamid of the team you want to add the deck to.", Example = typeof(DeckBaseDTOExampleGenerator))]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.Created, contentType: "application/json", bodyType: typeof(Guid), Description = "The ID of the deck that was created")]
        public async Task<IActionResult> CreateDeck(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequest req)
        {
            // Authentication
            Dictionary<string, string> auth = await _tokenService.GetRequestAuth(req);
            if (auth == null)
            {
                return new ObjectResult("No authentication was found or authentication was not a valid token") { StatusCode = 403 };
            }
            /*
            // Authorization - Role 
            bool personHasCorrectRole = _tokenService.CheckTeamRole(auth, Role.TeamLeader, deck.teamID);
            if (!personHasCorrectRole)
            {
                return new UnauthorizedResult();
            }*/

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            DeckBaseDTO deck;
            //trying to parse the json to the object, if unsuccesfull return bad request
            try
            {
                deck = JsonConvert.DeserializeObject<DeckBaseDTO>(requestBody);
            }
            catch (JsonSerializationException ex)
            {
                return new BadRequestObjectResult(ex.Message);
            }

            Guid deckID;
            try
            {
                deckID = await _deckService.CreateDeckAsync(deck);
            }
            catch (DbUpdateException)
            {
                return new InternalServerErrorResult();
            }

            return new OkObjectResult(deckID);
        }

        [FunctionName("UpdateDeck")]
        [Auth]
        [OpenApiOperation(operationId: "UpdateDeck", tags: new[] { "Deck" })]
        [OpenApiRequestBody("application/json", typeof(DeckBaseWithIDDTO), Description = "The deckID of the deck you want to update and the name you want the deck to have.", Example = typeof(DeckBaseWithIDDTOExampleGenerator))]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(string), Description = "The ok response")]
        public async Task<IActionResult> UpdateCard(
            [HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = null)] HttpRequest req)
        {
            // Authentication
            Dictionary<string, string> auth = await _tokenService.GetRequestAuth(req);
            if (auth == null)
            {
                return new ObjectResult("No authentication was found or authentication was not a valid token") { StatusCode = 403 };
            }

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            DeckBaseWithIDDTO deck;
            //trying to parse the json to the object, if unsuccesfull return bad request
            try
            {
                deck = JsonConvert.DeserializeObject<DeckBaseWithIDDTO>(requestBody);
            }
            catch (JsonSerializationException ex)
            {
                return new BadRequestObjectResult(ex.Message);
            }

            // Authorization - Role 
            bool personHasCorrectRole = _tokenService.CheckTeamRole(auth, Role.TeamLeader, deck.teamID);
            if (!personHasCorrectRole)
            {
                return new UnauthorizedResult();
            }

            try
            {
                await _deckService.UpdateDeckAsync(deck);
            }
            catch (EntityNotFoundException ex)
            {
                return new NotFoundObjectResult(ex.Message);
            }
            catch (DbUpdateException)
            {
                return new InternalServerErrorResult();
            }

            return new OkObjectResult("The deck has been updated.");
        }

        [FunctionName("GetDeck")]
        [Auth]
        [OpenApiOperation(operationId: "GetDeck", tags: new[] { "Deck" })]
        [OpenApiParameter(name: "DeckID", In = ParameterLocation.Query, Required = true, Type = typeof(Guid), Description = "The DeckID of the deck you want to get")]
        [OpenApiParameter(name: "TeamID", In = ParameterLocation.Query, Required = true, Type = typeof(Guid), Description = "The TeamID of the team the deck belongs to")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(CardDeck.Model.Entity.Deck), Description = "The deck that belongs to the given DeckID", Example = typeof(DeckDTOExampleGenerator))]
        public async Task<IActionResult> GetDeck(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = null)] HttpRequest req)
        {
            // Authentication
            Dictionary<string, string> auth = await _tokenService.GetRequestAuth(req);
            if (auth == null)
            {
                return new ObjectResult("No authentication was found or authentication was not a valid token") { StatusCode = 403 };
            }

            // Authorization - Everyone can use this endpoint

            Guid deckID;
            Guid teamID;
            //trying to parse the json to the object, if unsuccesfull return bad request
            try
            {
                deckID = Guid.Parse(req.Query["DeckID"]);
                teamID = Guid.Parse(req.Query["TeamID"]);
            }
            catch (JsonSerializationException ex)
            {
                return new BadRequestObjectResult(ex.Message);
            }

            CardDeck.Model.Entity.Deck deck;
            try
            {
                deck = await _deckService.GetDeckAsync(deckID, teamID);
            }
            catch (EntityNotFoundException ex)
            {
                return new NotFoundObjectResult(ex.Message);
            }
            catch (DbUpdateException)
            {
                return new InternalServerErrorResult();
            }

            return new OkObjectResult(deck);
        }

        [FunctionName("GetAllDecks")]
        [Auth]
        [OpenApiOperation(operationId: "GetAllDecks", tags: new[] { "Deck" })]
        [OpenApiParameter(name: "TeamID", In = ParameterLocation.Query, Required = true, Type = typeof(Guid), Description = "The TeamID of the team you want all the decks for.")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(List<DeckDTO>), Description = "All decks of a specific team", Example = typeof(DeckDTOExampleGenerator))]
        public async Task<IActionResult> GetAllDecks(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = null)] HttpRequest req)
        {
            // Authentication
            Dictionary<string, string> auth = await _tokenService.GetRequestAuth(req);
            if (auth == null)
            {
                return new ObjectResult("No authentication was found or authentication was not a valid token") { StatusCode = 403 };
            }
            /*
            // Authorization - Role 
            bool personHasCorrectRole = _tokenService.CheckTeamRole(auth, Role.TeamLeader, teamID);
            if (!personHasCorrectRole)
            {
                return new UnauthorizedResult();
            }
             */

            Guid teamID;
            //trying to parse the json to the object, if unsuccesfull return bad request
            try
            {
                teamID = Guid.Parse(req.Query["TeamID"]);
            }
            catch (JsonSerializationException ex)
            {
                return new BadRequestObjectResult(ex.Message);
            }

            List<CardDeck.Model.Entity.Deck> decks;
            try
            {
                decks = await _deckService.GetAllDecksAsync(teamID);
            }
            catch (EntityNotFoundException ex)
            {
                return new NotFoundObjectResult(ex.Message);
            }
            catch (DbUpdateException)
            {
                return new InternalServerErrorResult();
            }

            return new OkObjectResult(decks);
        }

        [FunctionName("DeleteDeck")]
        [Auth]
        [OpenApiOperation(operationId: "DeleteDeck", tags: new[] { "Deck" })]
        [OpenApiRequestBody("application/json", typeof(DeckWithDeckIDAndTeamIDDTO), Description = "The DeckID of the deck you want to delete", Example = typeof(DeckBaseWithDeckIDAndTeamIDDTOExampleGenerator))]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(string), Description = "The OK response")]
        public async Task<IActionResult> DeleteDeck(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequest req)
        {
            // Authentication
            Dictionary<string, string> auth = await _tokenService.GetRequestAuth(req);
            if (auth == null)
            {
                return new ObjectResult("No authentication was found or authentication was not a valid token") { StatusCode = 403 };
            }
            /*
                        // Authorization - Role 
            bool personHasCorrectRole = _tokenService.CheckTeamRole(auth, Role.TeamLeader, deck.TeamID);
            if (!personHasCorrectRole)
            {
                return new UnauthorizedResult();
            }*/

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            DeckWithDeckIDAndTeamIDDTO deck;
            try
            {
                deck = JsonConvert.DeserializeObject<DeckWithDeckIDAndTeamIDDTO>(requestBody);
            }
            catch (JsonSerializationException ex)
            {
                return new BadRequestObjectResult(ex.Message);
            }

            try
            {
                await _deckService.DeleteDeckFromTeamAsync(deck);
            }
            catch (EntityNotFoundException ex)
            {
                return new NotFoundObjectResult(ex.Message);
            }
            catch (DbUpdateException)
            {
                return new InternalServerErrorResult();
            }

            return new OkObjectResult("");
        }
    }
}

