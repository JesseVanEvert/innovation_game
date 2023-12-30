using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Abstractions;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Resolvers;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json;

namespace UserTeamOrg.Model.DTO
{
    public class OrganizationBaseWithIdDTO : OrganizationBaseDTO
    {
        [JsonRequired]
        public Guid OrganizationId { get; set; }
    }

    public class OrganizationBaseWithIdDTOExampleGenerator : OpenApiExample<OrganizationBaseWithIdDTO>
    {
        public override IOpenApiExample<OrganizationBaseWithIdDTO> Build(NamingStrategy NamingStrategy = null)
        {
            Examples.Add(OpenApiExampleResolver.Resolve("Organization 1", new OrganizationBaseWithIdDTO() { OrganizationId = Guid.NewGuid(), Name = "Organization 1" }, NamingStrategy));
            Examples.Add(OpenApiExampleResolver.Resolve("Organization 2", new OrganizationBaseWithIdDTO() { OrganizationId = Guid.NewGuid(), Name = "Organization 2" }, NamingStrategy));
            Examples.Add(OpenApiExampleResolver.Resolve("Organization 3", new OrganizationBaseWithIdDTO() { OrganizationId = Guid.NewGuid(), Name = "Organization 3" }, NamingStrategy));
            return this;
        }
    }

}
