
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Abstractions;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Resolvers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Game.BLL.Interfaces;
using Microsoft.AspNetCore.Http;
using System.Net.Http.Headers;
using Game.Models.Entity;

public class TokenIdentityValidationParameters : TokenValidationParameters {
	public TokenIdentityValidationParameters(string Issuer, string Audience, SymmetricSecurityKey SecurityKey) {
		RequireSignedTokens = true;
		ValidAudience = Audience;
		ValidateAudience = true;
		ValidIssuer = Issuer;
		ValidateIssuer = true;
		ValidateIssuerSigningKey = true;
		ValidateLifetime = true;
		IssuerSigningKey = SecurityKey;
		AuthenticationType = "Bearer";
	}
}

public class TokenService : ITokenService
{
	private ILogger Logger { get; }

	private string Issuer { get; }
	private string Audience { get; }
	private TimeSpan ValidityDuration { get; }

	private SigningCredentials Credentials { get; }
	private TokenIdentityValidationParameters ValidationParameters { get; }

	public TokenService(IConfiguration Configuration, ILogger<TokenService> Logger)
	{
		this.Logger = Logger;

        // TODO Get from configuration
        Issuer = Environment.GetEnvironmentVariable("JWT:Issuer");
        Audience = Environment.GetEnvironmentVariable("JWT:Audience");
        ValidityDuration = TimeSpan.FromDays(1);
        string Key = Environment.GetEnvironmentVariable("JWT:EncryptKey");

        SymmetricSecurityKey SecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Key));

		Credentials = new SigningCredentials(SecurityKey, SecurityAlgorithms.HmacSha256Signature);

		ValidationParameters = new TokenIdentityValidationParameters(Issuer, Audience, SecurityKey);
	}

	public async Task<ClaimsPrincipal?> GetByValue(string Value)
	{
		if (Value == null)
		{
			throw new Exception("No Token supplied");
		}

		JwtSecurityTokenHandler Handler = new JwtSecurityTokenHandler();

		try
		{
			SecurityToken ValidatedToken;
			ClaimsPrincipal Principal = Handler.ValidateToken(Value, ValidationParameters, out ValidatedToken);

			return await Task.FromResult(Principal);
		}
		catch (Exception e)
		{
			// TODO special exception
			return null;
		}
	}

    public async Task<Dictionary<string, string>?> GetRequestAuth(HttpRequest req)
    {
        Dictionary<string, string> rolesAndAccesslevel = new();

        string authorizationHeader = req.Headers["Authorization"];
        if (authorizationHeader == null)
        {
            return null;
        }

        AuthenticationHeaderValue bearerHeader = AuthenticationHeaderValue.Parse(authorizationHeader);
        ClaimsPrincipal? user = await GetByValue(bearerHeader.Parameter);

        if (user == null)
        {
            return null;
        }

        string[] standardClaims = { "nbf", "exp", "iat", "iss", "aud" };


        foreach (Claim claim in user.Claims)
        {
            if (!standardClaims.Any(s => claim.Type.Contains(s)))
            {
                rolesAndAccesslevel[claim.Type] = claim.Value;

            }
        }

        return rolesAndAccesslevel;
    }

    public bool CheckAccessLevel(Dictionary<string, string> auth, AccessLevel requiredAccessLevel)
    {
        if (auth == null || !auth.ContainsKey("Accesslevel"))
        {
            return false;
        }

        bool converted = Enum.TryParse(auth["Accesslevel"], out AccessLevel givenAccessLevel);

        // Higher number is lower level
        if (!converted || givenAccessLevel > requiredAccessLevel)
        {
            return false;
        }
        return true;
    }

    public bool CheckTeamRole(Dictionary<string, string> auth, Role requiredRole, Guid teamId)
    {
        if (auth == null || !auth.ContainsKey(teamId.ToString()))
        {
            return false;
        }

        bool converted = Enum.TryParse(auth[teamId.ToString()], out Role givenRole);

        // Higher number is lower level
        if (!converted || givenRole > requiredRole)
        {
            return false;
        }
        return true;
    }
}