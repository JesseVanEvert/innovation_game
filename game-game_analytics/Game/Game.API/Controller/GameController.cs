using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using Game.BLL.Interfaces;
using Game.Exceptions.Exceptions;
using Game.Models.DTO;
using Game.Models.DTO.Game;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Abstractions;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Enums;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Extensions;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Resolvers;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Game.API.Controller
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

    public class GameController
    {
        private ILogger Logger { get; }
        private readonly IGameService _gameService;
        private readonly ITokenService _tokenService;

        public GameController(ILogger<GameController> log, IGameService gameService, ITokenService tokenService)
        {
            Logger = log;
            _gameService = gameService;
            _tokenService = tokenService;
        }

        [FunctionName("CreateGame")]
        [OpenApiOperation(operationId: "CreateGame", tags: new[] { "Game" }, Summary = "Create new game", Description = "This API endpoint will generate a new game.")]
        [Auth]
        [OpenApiRequestBody("application/json", typeof(CreateGameBodyDTO), Description = "The game data.", Example = typeof(CreateGameBodyDTO))]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.Created, contentType: "application/json", bodyType: typeof(string), Description = "The OK response with the game guid.")]
        public async Task<IActionResult> CreateGame([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "Game/CreateGame")] HttpRequest req)
        {
            Dictionary<string, string> auth = await _tokenService.GetRequestAuth(req);
            if (auth == null)
            {
                return new ObjectResult("No authentication was found or authentication was not a valid token") { StatusCode = 403 };
            }

            Logger.LogInformation("Creating new game.");
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();

            try
            {
                CreateGameBodyDTO data = JsonConvert.DeserializeObject<CreateGameBodyDTO>(requestBody);
                Guid gameId = await _gameService.CreateGame(data);

                return new OkObjectResult(gameId.ToString());
            }
            catch (EntityNotFoundException ex)
            {
                return new BadRequestObjectResult(ex.Message);
            }
        }

        [FunctionName("JoinGame")]
        [OpenApiOperation(operationId: "JoinGame", tags: new[] { "Game" }, Summary = "Join a game", Description = "This API endpoint lets a user join a game temporarily")]
        [Auth]
        [OpenApiRequestBody("application/json", typeof(GameUserBodyDTO), Description = "The game data."/*, Example = typeof(GameUserBodyExampleDTOGenerator)*/)]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(string), Description = "The OK response with a message that confirms the user has joined the game")]
        public async Task<IActionResult> JoinGame([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "Game/JoinGame")] HttpRequest req)
        {
            Dictionary<string, string> auth = await _tokenService.GetRequestAuth(req);
            if (auth == null)
            {
                return new ObjectResult("No authentication was found or authentication was not a valid token") { StatusCode = 403 };
            }

            Logger.LogInformation("User joining a game.");

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();

            try
            {
                GameUserBodyDTO data = JsonConvert.DeserializeObject<GameUserBodyDTO>(requestBody);

                string message = await _gameService.JoinGame(data);

                return new OkObjectResult(message);
            }
            catch (EntityNotFoundException ex)
            {
                return new BadRequestObjectResult(ex.Message);
            }
            catch (Exception e)
            {
                return new OkObjectResult(e.Message);
            }
        }

        [FunctionName("GetPlayers")]
        [OpenApiOperation(operationId: "GetPlayers", tags: new[] { "Game" }, Summary = "Get the players", Description = "Poll this endpoint during the game to keep the players list updated")]
        [Auth]
        [OpenApiParameter(name: "GameID", In = ParameterLocation.Query, Required = true, Type = typeof(Guid), Description = "The GameID parameter")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(string), Description = "The OK response with the players")]
        public async Task<IActionResult> GetPlayers([HttpTrigger(AuthorizationLevel.Anonymous, "Get", Route = "Game/GetPlayers")] HttpRequest req)
        {
            Dictionary<string, string> auth = await _tokenService.GetRequestAuth(req);
            if (auth == null)
            {
                return new ObjectResult("No authentication was found or authentication was not a valid token") { StatusCode = 403 };
            }

            Logger.LogInformation("Retrieving all answers of a game.");

            try
            {
                Guid gameID = Guid.Parse(req.Query["GameID"]);

                HashSet<PlayerReponseDTO> players = await _gameService.GetCurrentPlayers(gameID);

                return new OkObjectResult(players);
            }
            catch (EntityNotFoundException ex)
            {
                return new BadRequestObjectResult(ex.Message);
            }
        }

        [FunctionName("ReadyPlayer")]
        [OpenApiOperation(operationId: "ReadyPlayer", tags: new[] { "Game" }, Summary = "Mark a player as ready", Description = "Use this endpoint to mark a player ready for the game or the next round")]
        [Auth]
        [OpenApiRequestBody("application/json", typeof(GameUserBodyDTO), Description = "The game data.", Example = typeof(GameUserBodyDTO))]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.Created, contentType: "application/json", bodyType: typeof(string), Description = "The OK response with the message.")]
        public async Task<IActionResult> ReadyPlayer([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "Game/ReadyPlayer")] HttpRequest req)
        {
            Dictionary<string, string> auth = await _tokenService.GetRequestAuth(req);
            if (auth == null)
            {
                return new ObjectResult("No authentication was found or authentication was not a valid token") { StatusCode = 403 };
            }

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();

            try
            {
                GameUserBodyDTO data = JsonConvert.DeserializeObject<GameUserBodyDTO>(requestBody);
                string message = await _gameService.PlayerIsReady(data);

                return new OkObjectResult(message);
            }
            catch (Exception ex)
            {
                return new BadRequestObjectResult(ex.Message);
            }
        }

        [FunctionName("HasGameEnded")]
        [OpenApiOperation(operationId: "HasGameEnded", tags: new[] { "Game" }, Summary = "Check if the game has ended", Description = "Call this endpoint before has checking if a new round has started")]
        [Auth]
        [OpenApiParameter(name: "GameID", In = ParameterLocation.Query, Required = true, Type = typeof(Guid), Description = "The GameID parameter")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(string), Description = "The OK response with the answer if the game has ended.")]
        public async Task<IActionResult> HasGameEnded([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "Game/HasGameEnded")] HttpRequest req)
        {
            Dictionary<string, string> auth = await _tokenService.GetRequestAuth(req);
            if (auth == null)
            {
                return new ObjectResult("No authentication was found or authentication was not a valid token") { StatusCode = 403 };
            }

            try
            {
                Guid gameID = Guid.Parse(req.Query["GameID"]);

                bool hasGameEnded = await _gameService.HasGameEnded(gameID);

                return new OkObjectResult(hasGameEnded);
            }
            catch (EntityNotFoundException ex)
            {
                return new BadRequestObjectResult(ex.Message);
            }
        }

        [FunctionName("HasNewRoundStarted")]
        [OpenApiOperation(operationId: "HasNewRoundStarted", tags: new[] { "Game" }, Summary = "Resolve to true if players are ready for the next round", Description = "Poll this endpoint after a player has scored all answers")]
        [Auth]
        [OpenApiParameter(name: "GameID", In = ParameterLocation.Query, Required = true, Type = typeof(Guid), Description = "The GameID parameter")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(string), Description = "The OK response with the answer if a new round has started")]
        public async Task<IActionResult> HasNewRoundStarted([HttpTrigger(AuthorizationLevel.Anonymous, "Get", Route = "Game/HasNewRoundStarted")] HttpRequest req)
        {
            Dictionary<string, string> auth = await _tokenService.GetRequestAuth(req);
            if (auth == null)
            {
                return new ObjectResult("No authentication was found or authentication was not a valid token") { StatusCode = 403 };
            }

            Logger.LogInformation("Retrieving best answers of a round.");

            try
            {
                Guid gameID = Guid.Parse(req.Query["GameID"]);

                bool hasNextRoundStarted = await _gameService.HasNewRoundStarted(gameID);

                return new OkObjectResult(hasNextRoundStarted);
            }
            catch (Exception ex)
            {
                return new BadRequestObjectResult(ex.Message);
            }
        }

        [FunctionName("GetCurrentCard")]
        [OpenApiOperation(operationId: "GetCurrentCard", tags: new[] { "Game" }, Summary = "Get card being played this round", Description = "Call this endpoint after a new round has started")]
        [Auth]
        [OpenApiParameter(name: "GameID", In = ParameterLocation.Query, Required = true, Type = typeof(Guid), Description = "The GameID parameter")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(string), Description = "The OK response with the current card")]
        public async Task<IActionResult> GetCurrentCard([HttpTrigger(AuthorizationLevel.Anonymous, "Get", Route = "Game/GetCurrentCard")] HttpRequest req)
        {
            Dictionary<string, string> auth = await _tokenService.GetRequestAuth(req);
            if (auth == null)
            {
                return new ObjectResult("No authentication was found or authentication was not a valid token") { StatusCode = 403 };
            }

            Logger.LogInformation("Retrieving best answers of a round.");

            try
            {
                Guid gameID = Guid.Parse(req.Query["GameID"]);

                CardResponseDTO currentCard = await _gameService.GetCurrentCard(gameID);

                return new OkObjectResult(currentCard);
            }
            catch (Exception ex)
            {
                return new BadRequestObjectResult(ex.Message);
            }
        }

        [FunctionName("ReceivedCard")]
        [OpenApiOperation(operationId: "ReceivedCard", tags: new[] { "Game" }, Summary = "Tell the backend a player has received the current card", Description = "This call is excuted so it is ensured everyone has the current card before starting a new round")]
        [Auth]
        [OpenApiRequestBody("application/json", typeof(GameUserBodyDTO), Description = "The game data.", Example = typeof(GameUserBodyDTO))]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.Created, contentType: "application/json", bodyType: typeof(string), Description = "The OK response with the message.")]
        public async Task<IActionResult> ReceivedCard([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "Game/ReceivedCard")] HttpRequest req)
        {
            Dictionary<string, string> auth = await _tokenService.GetRequestAuth(req);
            if (auth == null)
            {
                return new ObjectResult("No authentication was found or authentication was not a valid token") { StatusCode = 403 };
            }

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();

            try
            {
                GameUserBodyDTO data = JsonConvert.DeserializeObject<GameUserBodyDTO>(requestBody);
                await _gameService.ReceivedCard(data);

                return new OkObjectResult("Registered reception in the game session");
            }
            catch (EntityNotFoundException ex)
            {
                return new BadRequestObjectResult(ex.Message);
            }
            catch (Exception ex)
            {
                return new BadRequestObjectResult(ex.Message);
            }
        }

        [FunctionName("ShareAnswer")]
        [OpenApiOperation(operationId: "ShareAnswer", tags: new[] { "Game" }, Summary = "Share an answer", Description = "This API endpoint lets a user share an answer to a question")]
        [Auth]
        [OpenApiRequestBody("application/json", typeof(AnswerBaseDTO), Description = "An answer to a question"/*, Example = typeof(AnswerDTOExample)*/)]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(string), Description = "The OK response with a message that confirms the answer was received")]
        public async Task<IActionResult> ShareAnswer([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "Game/ShareAnswer")] HttpRequest req)
        {
            Dictionary<string, string> auth = await _tokenService.GetRequestAuth(req);
            if (auth == null)
            {
                return new ObjectResult("No authentication was found or authentication was not a valid token") { StatusCode = 403 };
            }

            Logger.LogInformation("User shares answer");

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();

            try
            {
                AnswerBaseDTO data = JsonConvert.DeserializeObject<AnswerBaseDTO>(requestBody);

                AnswerDTO answer = new()
                {
                    UserID = data.UserID,
                    GameId = data.GameId,
                    Answer = data.Answer,
                    AnswerID = Guid.NewGuid()
                };

                await _gameService.AddAnswer(answer);

                return new OkObjectResult("The answer has been registered");
            }
            catch (EntityNotFoundException ex)
            {
                return new BadRequestObjectResult(ex.Message);
            }
            catch (Exception ex)
            {
                return new BadRequestObjectResult(ex.Message);
            }
        }

        [FunctionName("GetRoundAnswers")]
        [OpenApiOperation(operationId: "GetRoundAnswers", tags: new[] { "Game" }, Summary = "Get the answers of the round", Description = "Call this endpoint after a player has given an answer")]
        [Auth]
        [OpenApiParameter(name: "GameID", In = ParameterLocation.Query, Required = true, Type = typeof(Guid), Description = "The GameID parameter")]
        [OpenApiParameter(name: "PlayerID", In = ParameterLocation.Query, Required = true, Type = typeof(Guid), Description = "The PlayerID parameter")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(string), Description = "The OK response with the answers of the round")]
        public async Task<IActionResult> GetRoundAnswers([HttpTrigger(AuthorizationLevel.Anonymous, "Get", Route = "Game/GetAnswersPerRound")] HttpRequest req)
        {
            Dictionary<string, string> auth = await _tokenService.GetRequestAuth(req);
            if (auth == null)
            {
                return new ObjectResult("No authentication was found or authentication was not a valid token") { StatusCode = 403 };
            }

            Logger.LogInformation("Retrieving all answers of a game.");

            try
            {
                Guid gameID = Guid.Parse(req.Query["GameID"]);
                Guid playerID = Guid.Parse(req.Query["PlayerID"]);

                HashSet<AnswerDTO> answers = await _gameService.GetRoundAnswers(gameID, playerID);

                return new OkObjectResult(answers);
            }
            catch (InvalidOperationException ex)
            {
                return new BadRequestObjectResult(ex.Message);
            }
        }

        [FunctionName("ScoreAnswer")]
        [OpenApiOperation(operationId: "ScoreAnswer", tags: new[] { "Game" }, Summary = "Score an answer", Description = "This API endpoint lets a user score an answer to a question")]
        [Auth]
        [OpenApiRequestBody("application/json", typeof(ScoreDTO), Description = "An answer to a question", Example = typeof(ScoreDTO))]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(string), Description = "The OK response with a message that confirms the score was received")]
        public async Task<IActionResult> ScoreAnswer([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "Game/ScoreAnswer")] HttpRequest req)
        {
            Dictionary<string, string> auth = await _tokenService.GetRequestAuth(req);
            if (auth == null)
            {
                return new ObjectResult("No authentication was found or authentication was not a valid token") { StatusCode = 403 };
            }

            Logger.LogInformation("User scores an answer");

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();

            try
            {
                ScoreDTO data = JsonConvert.DeserializeObject<ScoreDTO>(requestBody);

                await _gameService.ScoreAnswer(data);

                return new OkObjectResult("The score has been registered");
            }
            catch (EntityNotFoundException ex)
            {
                return new BadRequestObjectResult(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return new BadRequestObjectResult(ex.Message);
            }
        }

        [FunctionName("HasPlayerScoredAllAnswers")]
        [OpenApiOperation(operationId: "HasPlayerScoredAllAnswers", tags: new[] { "Game" }, Summary = "Resolves to true if player has answered all questions", Description = "Call this endpoint after a player has scored an answer. In the frontend the ready button should appear.")]
        [Auth]
        [OpenApiParameter(name: "PlayerID", In = ParameterLocation.Query, Required = true, Type = typeof(Guid), Description = "The PlayerID parameter")]
        [OpenApiParameter(name: "GameID", In = ParameterLocation.Query, Required = true, Type = typeof(Guid), Description = "The GameID parameter")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(string), Description = "The OK response with all answers of a particular round.")]
        public async Task<IActionResult> HasPlayerScoredAllQuestions([HttpTrigger(AuthorizationLevel.Anonymous, "Get", Route = "Game/HasPlayerScoredAllAnswers")] HttpRequest req)
        {
            Dictionary<string, string> auth = await _tokenService.GetRequestAuth(req);
            if (auth == null)
            {
                return new ObjectResult("No authentication was found or authentication was not a valid token") { StatusCode = 403 };
            }

            Logger.LogInformation("Retrieving best answers of a round.");

            try
            {
                Guid gameID = Guid.Parse(req.Query["GameID"]);
                Guid playerID = Guid.Parse(req.Query["PlayerID"]);

                bool hasPlayerScoredAllQuestions = await _gameService.HasPlayerScoredAllAnswers(playerID, gameID);

                return new OkObjectResult(hasPlayerScoredAllQuestions);
            }
            catch (Exception ex)
            {
                return new BadRequestObjectResult(ex.Message);
            }
        }

        [FunctionName("GetBestAnswersPerRound")]
        [OpenApiOperation(operationId: "GetBestAnswersPerRound", tags: new[] { "Game" }, Summary = "Get an x amount of best answers of a round", Description = "Poll this endpoint after a player has scored all the answers.")]
        [Auth]
        [OpenApiParameter(name: "GameID", In = ParameterLocation.Query, Required = true, Type = typeof(Guid), Description = "The GameID parameter")]
        [OpenApiParameter(name: "NumberOfAnswerToReturn", In = ParameterLocation.Query, Required = false, Type = typeof(int), Description = "Optional parameter with the amount of answers. Defaults to 0")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(string), Description = "The OK response with all answers of a particular round.")]
        public async Task<IActionResult> GetAnswersPerRound([HttpTrigger(AuthorizationLevel.Anonymous, "Get", Route = "Game/GetBestAnswersPerRound")] HttpRequest req)
        {
            Dictionary<string, string> auth = await _tokenService.GetRequestAuth(req);
            if (auth == null)
            {
                return new ObjectResult("No authentication was found or authentication was not a valid token") { StatusCode = 403 };
            }

            Logger.LogInformation("Retrieving best answers of a round.");

            try
            {
                Guid gameID = Guid.Parse(req.Query["GameID"]);
                int numberOfAnswerToReturn = int.Parse(req.Query["NumberOfAnswerToReturn"]);

                HashSet<AnswerResponseDTO> bestAnswers = await _gameService.GetBestAnswers(numberOfAnswerToReturn, gameID);

                return new OkObjectResult(bestAnswers);
            }
            catch (Exception ex)
            {
                return new BadRequestObjectResult(ex.Message);
            }
        }

        //Let a user leave a game
        [FunctionName("LeaveGame")]
        [OpenApiOperation(operationId: "LeaveGame", tags: new[] { "Game" }, Summary = "Leave a game", Description = "This API endpoint lets a user leave a game")]
        [Auth]
        [OpenApiRequestBody("application/json", typeof(GameUserBodyDTO), Description = "The game data.", Example = typeof(GameUserBodyDTO))]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(string), Description = "The OK response with a message that confirms the user has left the game")]
        public async Task<IActionResult> LeaveGame([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "Game/LeaveGame")] HttpRequest req)
        {
            Dictionary<string, string> auth = await _tokenService.GetRequestAuth(req);
            if (auth == null)
            {
                return new ObjectResult("No authentication was found or authentication was not a valid token") { StatusCode = 403 };
            }

            Logger.LogInformation("User leaving a game.");

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();

            try
            {
                GameUserBodyDTO data = JsonConvert.DeserializeObject<GameUserBodyDTO>(requestBody);

                string message = await _gameService.LeaveGame(data);

                return new OkObjectResult(message);
            }
            catch (EntityNotFoundException ex)
            {
                return new BadRequestObjectResult(ex.Message);
            }
        }

    }


}

