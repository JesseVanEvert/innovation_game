using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Abstractions;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Resolvers;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json;

namespace UserTeamOrg.Model.DTO
{
    public class TeamBaseWithIdDTO : TeamBaseDTO
    {
        [JsonRequired]
        public Guid TeamId { get; set; }
    }

    public class TeamBaseWithIdDTOExampleGenerator : OpenApiExample<TeamBaseWithIdDTO>
    {
        public override IOpenApiExample<TeamBaseWithIdDTO> Build(NamingStrategy NamingStrategy = null)
        {
            Examples.Add(OpenApiExampleResolver.Resolve("Team 1", new TeamBaseWithIdDTO() { TeamId = Guid.NewGuid(), Name = "Team 1", OrganizationId = Guid.NewGuid() }, NamingStrategy));
            Examples.Add(OpenApiExampleResolver.Resolve("Team 2", new TeamBaseWithIdDTO() { TeamId = Guid.NewGuid(), Name = "Team 2", OrganizationId = Guid.NewGuid() }, NamingStrategy));
            Examples.Add(OpenApiExampleResolver.Resolve("Team 3", new TeamBaseWithIdDTO() { TeamId = Guid.NewGuid(), Name = "Team 3", OrganizationId = Guid.NewGuid() }, NamingStrategy));
            return this;
        }
    }

}
