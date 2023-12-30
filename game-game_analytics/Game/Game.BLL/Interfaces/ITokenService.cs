using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using Game.Models.Entity;

namespace Game.BLL.Interfaces
{
    public interface ITokenService
    {
        Task<ClaimsPrincipal?> GetByValue(string Value);
        Task<Dictionary<string, string>?> GetRequestAuth(HttpRequest Request);
        bool CheckAccessLevel(Dictionary<string, string> auth, AccessLevel requiredAccessLevel);
        bool CheckTeamRole(Dictionary<string, string> auth, Role teamLeader, Guid teamId);
    }
}
