using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Abstractions;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Resolvers;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json;

namespace UserTeamOrg.Model.DTO
{
    public class TeamBaseDTO
    {
        [JsonRequired]
        public string Name { get; set; }
        [JsonRequired]
        public Guid OrganizationId { get; set; }

    }

    public class TeamBaseDTOExampleGenerator : OpenApiExample<TeamBaseDTO>
    {
        public override IOpenApiExample<TeamBaseDTO> Build(NamingStrategy NamingStrategy = null)
        {
            Examples.Add(OpenApiExampleResolver.Resolve("Team 1", new TeamBaseDTO() { Name = "Team 1", OrganizationId = Guid.NewGuid() }, NamingStrategy));
            Examples.Add(OpenApiExampleResolver.Resolve("Team 2", new TeamBaseDTO() { Name = "Team 2", OrganizationId = Guid.NewGuid() }, NamingStrategy));
            Examples.Add(OpenApiExampleResolver.Resolve("Team 3", new TeamBaseDTO() { Name = "Team 3", OrganizationId = Guid.NewGuid() }, NamingStrategy));
            return this;
        }
    }

}
