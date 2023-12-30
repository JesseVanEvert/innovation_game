using Game.Models.DTO;
using Game.Models.DTO.Game;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game.BLL.Interfaces
{
    public interface IGameService
    {
        Task<Guid> CreateGame(CreateGameBodyDTO data);
        Task<string> JoinGame(GameUserBodyDTO gameUser);
        Task<bool> HasNewRoundStarted(Guid gameID);
        Task<CardResponseDTO> GetCurrentCard(Guid gameID);
        Task ReceivedCard(GameUserBodyDTO player);
        Task AddAnswer(AnswerDTO answer);
        Task<HashSet<PlayerReponseDTO>> GetCurrentPlayers(Guid gameID);
        Task<HashSet<AnswerDTO>> GetRoundAnswers(Guid gameID, Guid userID);
        Task ScoreAnswer(ScoreDTO score);
        Task<bool> HasPlayerScoredAllAnswers(Guid playerID, Guid gameID);
        Task<HashSet<AnswerResponseDTO>> GetBestAnswers(int numberOfAnswerToReturn, Guid gameID);
        Task<string> PlayerIsReady(GameUserBodyDTO readyDTO);
        Task<bool> HasGameEnded(Guid gameID);
        Task<string> LeaveGame(GameUserBodyDTO gameUser);
    }
}
