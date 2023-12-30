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

namespace Card.API.Controller
{
    public class CardControllers
    {
        public class AuthAttribute : OpenApiSecurityAttribute
        {
            public AuthAttribute() : base("Auth", SecuritySchemeType.Http)
            {
                Description = "JWT for authorization and authorization";
                In = OpenApiSecurityLocationType.Header;
                Scheme = OpenApiSecuritySchemeType.Bearer;
                BearerFormat = "JWT";
            }
        }

        private readonly ILogger<CardControllers> _logger;
        private ICardService _cardService;
        private ITokenService _tokenService;

        public CardControllers(ILogger<CardControllers> log, ICardService cardService, ITokenService tokenService)
        {
            _logger = log;
            _cardService = cardService;
            _tokenService = tokenService;
        }

        [FunctionName("CreateCardInDeck")]
        [Auth]
        [OpenApiOperation(operationId: "CreateCardInDeck", tags: new[] { "Card" })]
        [OpenApiRequestBody("application/json", typeof(CardBaseWithDeckIDDTO), Description = "All the information necessary to create a new card, including the DeckID to which the card should be added.", Example = typeof(CardBaseWithDeckIDDTOExampleGenerator))]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.Created, contentType: "application/json", bodyType: typeof(Guid), Description = "The ID of the card that was made")]
        public async Task<IActionResult> CreateCard(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequest req)
        {
            // Authentication
            Dictionary<string, string> auth = await _tokenService.GetRequestAuth(req);
            if (auth == null)
            {
                return new ObjectResult("No authentication was found or authentication was not a valid token") { StatusCode = 403 };
            }

            // Authorization - Everyone can access this endpoint

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            CardBaseWithDeckIDDTO cardBase;
            //trying to parse the json to the object, if unsuccesfull return bad request
            try
            {
                cardBase = JsonConvert.DeserializeObject<CardBaseWithDeckIDDTO>(requestBody);
            }
            catch (JsonSerializationException ex)
            {
                return new BadRequestObjectResult(ex.Message);
            }

            Guid cardID;
            try
            {
                cardID = await _cardService.CreateCardInDeckAsync(cardBase);
            }
            catch (EntityNotFoundException ex)
            {
                return new NotFoundObjectResult(ex.Message);
            }
            catch (DbUpdateException)
            {
                return new InternalServerErrorResult();
            }


            return new OkObjectResult(cardID);
        }

        [FunctionName("UpdateCard")]
        [Auth]
        [OpenApiOperation(operationId: "UpdateCard", tags: new[] { "Card" })]
        [OpenApiRequestBody("application/json", typeof(CardBaseWithCardIDDTO), Description = "All the information necessary for a card, any updated properties will be updated in the database. CardID used for finding the right card to update.", Example = typeof(CardBaseWithCardIDDTOExampleGenerator))]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(string), Description = "The card has been updated")]
        public async Task<IActionResult> UpdateCard(
            [HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = null)] HttpRequest req)
        {
            // Authentication
            Dictionary<string, string> auth = await _tokenService.GetRequestAuth(req);
            if (auth == null)
            {
                return new ObjectResult("No authentication was found or authentication was not a valid token") { StatusCode = 403 };
            }

            // Authorization - Everyone can access this endpoint

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            CardBaseWithCardIDDTO cardBase;
            //trying to parse the json to the object, if unsuccesfull return bad request
            try
            {
                cardBase = JsonConvert.DeserializeObject<CardBaseWithCardIDDTO>(requestBody);
            }
            catch (JsonSerializationException ex)
            {
                return new BadRequestObjectResult(ex.Message);
            }

            try
            {
                await _cardService.UpdateCardInDeckAsync(cardBase);
            }
            catch (EntityNotFoundException ex)
            {
                return new NotFoundObjectResult(ex.Message);
            }
            catch (DbUpdateException)
            {
                return new InternalServerErrorResult();
            }


            return new OkObjectResult($"Updated card");
        }

        [FunctionName("GetCardFromDeck")]
        [Auth]
        [OpenApiOperation(operationId: "GetCardFromDeck", tags: new[] { "Card" })]
        [OpenApiParameter(name: "CardID", In = ParameterLocation.Query, Required = true, Type = typeof(Guid), Description = "The cardID of a card you want to receive")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(CardDeck.Model.Entity.Card), Description = "The card information", Example = typeof(CardDTOExampleGenerator))]
        public async Task<IActionResult> GetCardFromDeck(
            [HttpTrigger(AuthorizationLevel.Anonymous, "Get", Route = null)] HttpRequest req)
        {
            // Authentication
            Dictionary<string, string> auth = await _tokenService.GetRequestAuth(req);
            if (auth == null)
            {
                return new ObjectResult("No authentication was found or authentication was not a valid token") { StatusCode = 403 };
            }

            // Authorization - Everyone can use this endpoint

            Guid cardID;
            //trying to parse the json to the object, if unsuccesfull return bad request
            try
            {
                cardID = Guid.Parse(req.Query["CardID"]);
            }
            catch (JsonSerializationException ex)
            {
                return new BadRequestObjectResult(ex.Message);
            }

            CardDeck.Model.Entity.Card card;
            try
            {
                card = await _cardService.GetCardFromDeckAsync(cardID);
            }
            catch (EntityNotFoundException ex)
            {
                return new NotFoundObjectResult(ex.Message);
            }
            catch (DbUpdateException)
            {
                return new InternalServerErrorResult();
            }

            return new OkObjectResult(card);
        }

        [FunctionName("DeleteCardFromDeck")]
        [Auth]
        [OpenApiOperation(operationId: "DeleteCardFromDeck", tags: new[] { "Card" })]
        [OpenApiRequestBody("application/json", typeof(CardWithCardIDAndDeckIDDTO), Description = "The DeckID and CardID of the card you want to delete", Example = typeof(CardWithCardIDAndDeckIDDTOExampleGenerator))]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(string), Description = "The card has been deleted")]
        public async Task<IActionResult> DeleteCardFromDeck(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequest req)
        {
            // Authentication
            Dictionary<string, string> auth = await _tokenService.GetRequestAuth(req);
            if (auth == null)
            {
                return new ObjectResult("No authentication was found or authentication was not a valid token") { StatusCode = 403 };
            }

            // Authorization - Everyone can access this endpoint

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            CardWithCardIDAndDeckIDDTO cardBase;
            //trying to parse the json to the object, if unsuccesfull return bad request
            try
            {
                cardBase = JsonConvert.DeserializeObject<CardWithCardIDAndDeckIDDTO>(requestBody);
            }
            catch (JsonSerializationException ex)
            {
                return new BadRequestObjectResult(ex.Message);
            }
            try
            {
                await _cardService.DeleteCardInDeckAsync(cardBase);
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

        //Get all cards from deck
        [FunctionName("GetAllCardsFromDeck")]
        [Auth]
        [OpenApiOperation(operationId: "GetAllCardsFromDeck", tags: new[] { "Card" })]
        [OpenApiParameter(name: "DeckID", In = ParameterLocation.Query, Required = true, Type = typeof(Guid), Description = "The deckID of a deck you want to receive all cards from")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(List<CardDeck.Model.Entity.Card>), Description = "All the cards in the deck", Example = typeof(CardDTOExampleGenerator))]
        public async Task<IActionResult> GetAllCardsFromDeck(
            [HttpTrigger(AuthorizationLevel.Anonymous, "Get", Route = null)] HttpRequest req)
        {
            // Authentication
            Dictionary<string, string> auth = await _tokenService.GetRequestAuth(req);
            if (auth == null)
            {
                return new ObjectResult("No authentication was found or authentication was not a valid token") { StatusCode = 403 };
            }

            // Authorization - Everyone can use this endpoint

            Guid deckID;
            //trying to parse the json to the object, if unsuccesfull return bad request
            try
            {
                deckID = Guid.Parse(req.Query["DeckID"]);
            }
            catch (JsonSerializationException ex)
            {
                return new BadRequestObjectResult(ex.Message);
            }

            List<CardDeck.Model.Entity.Card> cards;
            try
            {
                cards = await _cardService.GetAllCardsFromDeckAsync(deckID);
            }
            catch (EntityNotFoundException ex)
            {
                return new NotFoundObjectResult(ex.Message);
            }
            catch (DbUpdateException)
            {
                return new InternalServerErrorResult();
            }

            return new OkObjectResult(cards);
        }
    }
}

