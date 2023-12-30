using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Abstractions;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Resolvers;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserTeamOrg.Model.DTO
{
    public class LoginResult
    {
        private JwtSecurityToken Token { get; }

        [OpenApiProperty(Description = "The access token to be used in every subsequent operation for this user.")]
        [JsonRequired]
        public string AccessToken => new JwtSecurityTokenHandler().WriteToken(Token);

        [OpenApiProperty(Description = "The token type.")]
        [JsonRequired]
        public string TokenType => "Bearer";

        [OpenApiProperty(Description = "The amount of seconds until the token expires.")]
        [JsonRequired]
        public int ExpiresIn => (int)(Token.ValidTo - DateTime.UtcNow).TotalSeconds;

        public LoginResult(JwtSecurityToken Token)
        {
            this.Token = Token;
        }
    }
}
