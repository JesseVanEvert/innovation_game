using Game.BLL.Interfaces;
using Game.Models.DTO;
using Microsoft.Extensions.Caching.Distributed;
using Game.BLL.Extensions;
using Game.Models.Entity;
using Game.DAL;
using Game.DAL.Repository;
using Game.DAL.Interfaces;
using Game.Exceptions.Exceptions;
using Game.Models.DTO.Game;

namespace Game.BLL
{
    public class GameService : IGameService
    {
        private readonly IDistributedCache _cache;
        private readonly IDeckRepository _deckRepository;
        private readonly IUserRepository _userRepository;
        private readonly IGameRepository _gameRepository;
        public GameService(IDistributedCache cache, IDeckRepository deckRepository,
            IUserRepository userRepository, IGameRepository gameRepository)
        {
            _cache = cache;
            _deckRepository = deckRepository;
            _userRepository = userRepository;
            _gameRepository = gameRepository;
        }

        public async Task<Guid> CreateGame(CreateGameBodyDTO data)
        {
            Guid gameId = Guid.NewGuid();

            Models.Entity.Game game = new()
            {
                GameID = gameId,
                Name = data.Name,
                Start = DateTime.Now
            };

            List<Card> cards = await _deckRepository.GetAllCardsFromDeckAsync(data.DeckID) 
                ?? throw new EntityNotFoundException("Deck not found");
            Task recordNameTask = _cache.SetRecordAsync($"game_{gameId}", game);
            Task recordDeckTask = _cache.SetRecordAsync($"game_{gameId}_cards", cards);

            Task.WaitAll(recordNameTask, recordDeckTask);

            await _gameRepository.CreateGame(game);

            return gameId;
        }

        public async Task<string> JoinGame(GameUserBodyDTO gameUser)
        {
            User user = await _userRepository.GetUserAsync(gameUser.UserID)
                ?? throw new EntityNotFoundException("User not found");

            List<User> players = await _cache
                .GetRecordAsync<List<User>>($"game_{gameUser.GameID}_players")
                ?? new List<User>();

            User existingPlayer = players.SingleOrDefault(ep => ep.UserID == gameUser.UserID);

            if (existingPlayer != null)
                return "Player already in game";

            players.Add(user);

            await _cache.SetRecordAsync($"game_{gameUser.GameID}_players", players);
            await _gameRepository.AddGameUserToGame(gameUser);

            return $"Player {user.Firstname} added to game";
        }

        //Is polled the whole game after joining
        public async Task<HashSet<PlayerReponseDTO>> GetCurrentPlayers(Guid gameID)
        {
            Task<HashSet<User>> playersTask = _cache.GetRecordAsync<HashSet<User>>($"game_{gameID}_players");
            Task<HashSet<Guid>> readyPlayersTask = _cache.GetRecordAsync<HashSet<Guid>>($"game_{gameID}_readyPlayers");

            HashSet<User> players = await playersTask ?? throw new EntityNotFoundException("No players found");
            HashSet<Guid> readyPlayers = await readyPlayersTask;

            HashSet<PlayerReponseDTO> playerReponses = new();

            foreach (var player in players)
            {
                PlayerReponseDTO playerResponse = new();
                playerResponse.PlayerName = player.Firstname;
                if (readyPlayers != null)
                {
                    foreach (var readyPlayer in readyPlayers)
                    {
                        if (player.UserID == readyPlayer)
                            playerResponse.Ready = true;
                    }
                }
                playerReponses.Add(playerResponse);
            }

            return playerReponses;
        }

        public async Task<string> PlayerIsReady(GameUserBodyDTO readyDTO)
        {
            Task<HashSet<User>> playersTask = _cache.GetRecordAsync<HashSet<User>>($"game_{readyDTO.GameID}_players");
            Task<HashSet<Guid>> readyPlayersTask = _cache.GetRecordAsync<HashSet<Guid>>($"game_{readyDTO.GameID}_readyPlayers");

            HashSet<User> players = await playersTask;

            User readyPlayer = players.SingleOrDefault(p => p.UserID == readyDTO.UserID)
                ?? throw new EntityNotFoundException("Player has not been found");

            HashSet<Guid> readyPlayers = await readyPlayersTask;

            if (readyPlayers == null)
                readyPlayers = new();

            bool playerAlreadyReady = readyPlayers.TryGetValue(readyDTO.UserID, out Guid item);
            if (playerAlreadyReady)
                return "Player is already ready";

            readyPlayers.Add(readyDTO.UserID);
            await _cache.SetRecordAsync($"game_{readyDTO.GameID}_readyPlayers", readyPlayers);

            return $"Player {readyPlayer.Firstname} is ready";
        }

        //Is called before HasNewRoundStarted
        public async Task<bool> HasGameEnded(Guid gameID)
        {
            HashSet<Card> cards = await _cache.GetRecordAsync<HashSet<Card>>($"game_{gameID}_cards");

            if (!cards.Any())
            {
                await _gameRepository.UpdateGameWithEndDate(gameID);
                return true;
            }

            return false;
        }

        //Is polled when a player is set as ready. Stops polling if the call resolves in true.
        public async Task<bool> HasNewRoundStarted(Guid gameID)
        {
            Task<HashSet<User>> playersTask = _cache.GetRecordAsync<HashSet<User>>($"game_{gameID}_players");
            Task<HashSet<Guid>> readyPlayersTask = _cache.GetRecordAsync<HashSet<Guid>>($"game_{gameID}_readyPlayers");

            HashSet<User> players = await playersTask
                ?? throw new EntityNotFoundException("No players have been found");
            HashSet<Guid> readyPlayers = await readyPlayersTask;


            if (readyPlayers == null)
                return false;

            if (players.Count == readyPlayers.Count)
            {
                await _cache.RemoveAsync($"game_{gameID}_answers");
                await _cache.RemoveAsync($"game_{gameID}_scores");

                return true;
            }

            return false;
        }

        //Is called after HasNewRoundStarted resolves in true
        public async Task<CardResponseDTO> GetCurrentCard(Guid gameID)
        {
            HashSet<Card> cards = await _cache.GetRecordAsync<HashSet<Card>>($"game_{gameID}_cards");

            return new CardResponseDTO()
            {
                FrontSideText = cards.First().FrontSideText,
                BackSideText = cards.First().BackSideText,
                ImageUrl= cards.First().ImageUrl,
            };
        }

        //Is called after a card has been received.
        //Only after everyone has received the current card the first card is removed. 
        public async Task ReceivedCard(GameUserBodyDTO player)
        {
            Task<HashSet<Guid>> playersWhoReceivedCurrentCardTask = _cache.GetRecordAsync<HashSet<Guid>>($"game_{player.GameID}_players_who_received_current_card");
            Task<HashSet<Guid>> readyPlayersTask = _cache.GetRecordAsync<HashSet<Guid>>($"game_{player.GameID}_readyPlayers");
            Task<HashSet<Card>> cardsTask = _cache.GetRecordAsync<HashSet<Card>>($"game_{player.GameID}_cards");

            HashSet<Guid> playersWhoReceivedCurrentCard = await playersWhoReceivedCurrentCardTask ?? new();
            bool playerAlreadyReceivedCard = playersWhoReceivedCurrentCard.TryGetValue(player.UserID, out Guid item);
            if (playerAlreadyReceivedCard)
                throw new Exception("Player already received card");

            playersWhoReceivedCurrentCard.Add(player.UserID);
            await _cache.SetRecordAsync($"game_{player.GameID}_players_who_received_current_card", playersWhoReceivedCurrentCard);

            HashSet<Guid> readyPlayers = await readyPlayersTask
                ?? throw new EntityNotFoundException("No ready players found");
            HashSet<Card> cards = await cardsTask
                ?? throw new EntityNotFoundException("No cards have been found");

            if (playersWhoReceivedCurrentCard.Count == readyPlayers.Count)
            {
                await _cache.RemoveAsync($"game_{player.GameID}_players_who_received_current_card");
                await _cache.RemoveAsync($"game_{player.GameID}_readyPlayers");
                cards.Remove(cards.First());
                await _cache.SetRecordAsync($"game_{player.GameID}_cards", cards);
            }
        }

        public async Task AddAnswer(AnswerDTO answer)
        {
            Task<HashSet<User>> playersTask = _cache.GetRecordAsync<HashSet<User>>($"game_{answer.GameId}_players");
            HashSet<Card> cards = await _cache.GetRecordAsync<HashSet<Card>>($"game_{answer.GameId}_cards")
                ?? throw new EntityNotFoundException("Cards have not been found");

            Guid currentCardId = cards.First().CardID;

            HashSet<User> players = await playersTask;
            User playerWhoAnswered = players.SingleOrDefault(p => p.UserID == answer.UserID)
                ?? throw new EntityNotFoundException("Player has not been found");

            HashSet<AnswerDTO> answers = await _cache
                .GetRecordAsync<HashSet<AnswerDTO>>($"game_{answer.GameId}_answers") ?? new();

            foreach (var a in answers)
            {
                if (a.UserID == playerWhoAnswered.UserID)
                    throw new Exception("Player has already answered this round");
            }

            answers.Add(answer);

            await _cache.SetRecordAsync($"game_{answer.GameId}_answers", answers);
        }

        //Is polled after sharing an answer. Polling stops after all answers have been scored by the player.
        public async Task<HashSet<AnswerDTO>> GetRoundAnswers(Guid gameID, Guid userID)
        {
            HashSet<AnswerDTO> answers = await _cache
                .GetRecordAsync<HashSet<AnswerDTO>>($"game_{gameID}_answers")
                ?? throw new InvalidOperationException("No answers have been found");

            HashSet<AnswerDTO> answersForUser = new();
            foreach (var answer in answers)
            {
                if (answer.UserID != userID)
                    answersForUser.Add(answer);
            }

            return answersForUser;
        }

        public async Task ScoreAnswer(ScoreDTO score)
        {
            Task<HashSet<ScoreDTO>> scoresTask = _cache.GetRecordAsync<HashSet<ScoreDTO>>($"game_{score.GameID}_scores");
            Task<HashSet<AnswerDTO>> answersTask = _cache.GetRecordAsync<HashSet<AnswerDTO>>($"game_{score.GameID}_answers");
            Task<HashSet<Card>> cardsTask = _cache.GetRecordAsync<HashSet<Card>>($"game_{score.GameID}_cards");

            HashSet<AnswerDTO> answers = await answersTask
                ?? throw new EntityNotFoundException("Answers have not been found");
            AnswerDTO answer = answers.SingleOrDefault(a => a.AnswerID == score.AnswerID)
                ?? throw new EntityNotFoundException("Answer has not been found");

            HashSet<ScoreDTO> scores = await scoresTask ?? new();

            //Is the player trying to score her own answer?
            AnswerDTO answerOfGrader = answers.SingleOrDefault(ep => ep.UserID == score.UserID)
                ?? throw new EntityNotFoundException("Player hasn't given an answers");
            if (score.AnswerID == answerOfGrader.AnswerID)
                throw new InvalidOperationException("Players can't score their own answer");

            //Has player already scored the answer
            foreach (var s in scores)
            {
                if (s.UserID == score.UserID && s.AnswerID == score.AnswerID)
                    throw new InvalidOperationException("Player has already scored the answer");
            }

            scores.Add(score);

            HashSet<Card> cards = await cardsTask;
            Card card = cards.First();

            await _cache.SetRecordAsync($"game_{score.GameID}_scores", scores);
        }

        //Is called every time after a player gives a score
        public async Task<bool> HasPlayerScoredAllAnswers(Guid playerID, Guid gameID)
        {
            Task<HashSet<AnswerDTO>> answersTask = _cache.GetRecordAsync<HashSet<AnswerDTO>>($"game_{gameID}_answers");
            Task<HashSet<ScoreDTO>> scoresTask = _cache.GetRecordAsync<HashSet<ScoreDTO>>($"game_{gameID}_scores");

            HashSet<ScoreDTO> scores = await scoresTask
                ?? throw new InvalidOperationException("No scores have been found");
            HashSet<AnswerDTO> answers = await answersTask
                ?? throw new InvalidOperationException("No answers have been found");

            //Get the scores the player has given
            HashSet<ScoreDTO> scoresGiven = scores.Where(s => s.UserID == playerID).ToHashSet();

            //Minus the answer the player has given herself
            return scoresGiven.Count == (answers.Count - 1);
        }


        //Is polled after HasPlayerScoredAllAnswers resolves to true. Polling stops after the number of answers to return has been met, ready button has to appear.
        public async Task<HashSet<AnswerResponseDTO>> GetBestAnswers(int numberOfAnswerToReturn, Guid gameID)
        {
            Task<HashSet<AnswerDTO>> answersTask = _cache.GetRecordAsync<HashSet<AnswerDTO>>($"game_{gameID}_answers");
            Task<HashSet<ScoreDTO>> scoresTask = _cache.GetRecordAsync<HashSet<ScoreDTO>>($"game_{gameID}_scores");
            Task<HashSet<User>> playersTask = _cache.GetRecordAsync<HashSet<User>>($"game_{gameID}_players");
            Task<HashSet<Card>> cardsTask = _cache.GetRecordAsync<HashSet<Card>>($"game_{gameID}_cards");

            HashSet<ScoreDTO> scores = await scoresTask 
                ?? throw new InvalidOperationException("No scores have been found");
            HashSet<ScoreDTO> topScores = scores.OrderByDescending(s => s.Score).Take(numberOfAnswerToReturn).ToHashSet();

            HashSet<AnswerDTO> answers = await answersTask
                ?? throw new InvalidOperationException("No answers have been found");

            HashSet<User> players = await playersTask
                ?? throw new InvalidOperationException("No players have been found");

            HashSet<AnswerResponseDTO> topAnswers = CreateTopAnswersRepsonseCollection(topScores, answers, players);

            return topAnswers;
        }

        private static HashSet<AnswerResponseDTO> CreateTopAnswersRepsonseCollection(HashSet<ScoreDTO> topScores, HashSet<AnswerDTO> answers, HashSet<User> players)
        {
            HashSet<AnswerResponseDTO> topAnswerResponses = new();

            foreach(var player in players)
            {
                foreach(var answer in answers)
                {
                    foreach (var score in topScores)
                    { 
                        if (answer.AnswerID == score.AnswerID && answer.UserID == player.UserID)
                        {
                            topAnswerResponses.Add(new AnswerResponseDTO()
                            {
                                PlayerName = $"{player.Firstname} {player.Lastname}",
                                Answer = answer.Answer,
                                Score = score.Score
                            });
                        }
                    }       
                }
            }

            return topAnswerResponses;
        }

        public async Task<string> LeaveGame(GameUserBodyDTO gameUser)
        {
            List<User> players = await _cache.GetRecordAsync<List<User>>($"game_{gameUser.GameID}_players") 
                ?? throw new EntityNotFoundException("No players found");

            User user = players.SingleOrDefault(p => p.UserID == gameUser.UserID)
                ?? throw new EntityNotFoundException("Player not found");

            players.Remove(user);
            await _cache.SetRecordAsync($"game_{gameUser.GameID}_players", players);

            await _gameRepository.StoreLeaveDateTimeOfPlayer(gameUser);

            return "Player left game";
        }
    }
}

