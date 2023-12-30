using Game.DAL.Interfaces;
using Game.Exceptions.Exceptions;
using Game.Models.DTO;
using Game.Models.DTO.Game;
using Game.Models.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game.DAL.Repository
{
    public class GameRepository : IGameRepository
    {
        private readonly GameContext _gameContext;
        public GameRepository(GameContext gameContext)
        {
            _gameContext = gameContext;
        }

        public async Task CreateGame(Models.Entity.Game game)
        {
            await _gameContext.Games.AddAsync(game);
            await _gameContext.SaveChangesAsync();
        }

        public async Task AddGameUserToGame(GameUserBodyDTO playerInfo)
        {
            GameUser player = new()
            {
                UserID = playerInfo.UserID,
                GameID = playerInfo.GameID,
                Joined = DateTime.Now,
            };

            Models.Entity.Game game = await _gameContext.Games.FindAsync(player.GameID) 
                ?? throw new EntityNotFoundException("Game not found");

            try {
                game.GameUsers.Add(player);
                await _gameContext.SaveChangesAsync();
            } 
            catch
            {
                throw new Exception("Player rejoined the game");
            }
        }

        /* 
         * This function is not yet functional, and should be implemented when 
         * the game analytics module is added. It is not needed for playing the game only for analytics.
         */
        public async Task AddAnswerToPlayer(AnswerDTO answerInfo)
        {
            GameUser player = await _gameContext.GameUsers.FindAsync(answerInfo.UserID, answerInfo.GameId)
                ?? throw new EntityNotFoundException("Player not found");

            GameUserAnswer answer = new()
            {
                GameUserAnswerID = Guid.NewGuid(),
                Answer = answerInfo.Answer,
                GameUser = player,
                UserID = player.UserID
            };

            player.GameUserAnswers.Add(answer);
            await _gameContext.SaveChangesAsync();
        }

        /* 
         * This function is not yet functional, and should be implemented when 
         * the game analytics module is added. It is not needed for playing the game only for analytics.
         */
        public async Task AddScoreToAnswer(ScoreDTO score, Card card)
        {
            GameUserAnswer answer = await _gameContext.GameUsersAnswers.FindAsync(score.AnswerID)
                ?? throw new EntityNotFoundException("Answer not found");

            GameUserScore answerScore = new()
            {
                GameUserAnswerID = answer.GameUserAnswerID,
                CardID = card.CardID,
                Score = score.Score,
                Card = card,
                GameUserAnswer = answer
            };

            answer.GameUserScores.Add(answerScore);
            await _gameContext.SaveChangesAsync();
        }

        public async Task UpdateGameWithEndDate(Guid gameID)
        {
            Models.Entity.Game game = await _gameContext.Games.FindAsync(gameID) 
                ?? throw new EntityNotFoundException("Game not found");

            game.End = DateTime.Now;
            await _gameContext.SaveChangesAsync();
        }

        public async Task StoreLeaveDateTimeOfPlayer(GameUserBodyDTO playerInfo)
        {
            GameUser oldPlayer = await _gameContext.GameUsers.FindAsync(playerInfo.UserID, playerInfo.GameID)
                ?? throw new EntityNotFoundException("Player not found");

            GameUser newPlayer = oldPlayer;
            newPlayer.Left = DateTime.Now;

            _gameContext.Entry(oldPlayer).CurrentValues.SetValues(newPlayer);
            await _gameContext.SaveChangesAsync();
        }
    }
}
