using Game.Models.DTO;
using Game.Models.DTO.Game;
using Game.Models.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game.DAL.Interfaces
{
    public interface IGameRepository
    {
        Task CreateGame(Models.Entity.Game game);
        Task AddGameUserToGame(GameUserBodyDTO playerInfo);
        Task StoreLeaveDateTimeOfPlayer(GameUserBodyDTO playerInfo);
        Task AddAnswerToPlayer(AnswerDTO answerInfo);
        Task AddScoreToAnswer(ScoreDTO score, Card card);
        Task UpdateGameWithEndDate(Guid gameID);
    }
}
