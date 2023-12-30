using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using UserTeamOrg.Model;
using UserTeamOrg.Model.DTO;
using UserTeamOrg.Model.Entity;

namespace UserTeamOrg.BLL.Interfaces
{
    public interface ITokenService
    {
        Task<LoginResult> CreateToken(Dictionary<string, string> RolesAndAccessLevel);
        Task<ClaimsPrincipal?> GetByValue(string Value);
        Task<Dictionary<string, string>?> GetRequestAuth(HttpRequest Request);
        bool CheckAccessLevel(Dictionary<string, string> auth, AccessLevel requiredAccessLevel);
        bool CheckTeamRole(Dictionary<string, string> auth, Role teamLeader, Guid teamId);
    }
}
