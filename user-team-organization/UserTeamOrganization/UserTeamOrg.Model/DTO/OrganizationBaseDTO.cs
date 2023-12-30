using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Abstractions;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Resolvers;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json;

namespace UserTeamOrg.Model.DTO
{
    public class OrganizationBaseDTO
    {
        [JsonRequired]
        public string Name { get; set; }
    }

    public class OrganizationBaseDTOExampleGenerator : OpenApiExample<OrganizationBaseDTO>
    {
        public override IOpenApiExample<OrganizationBaseDTO> Build(NamingStrategy NamingStrategy = null)
        {
            Examples.Add(OpenApiExampleResolver.Resolve("Organization 1", new OrganizationBaseDTO() { Name = "Organization 1" }, NamingStrategy));
            Examples.Add(OpenApiExampleResolver.Resolve("Organization 2", new OrganizationBaseDTO() { Name = "Organization 2" }, NamingStrategy));
            Examples.Add(OpenApiExampleResolver.Resolve("Organization 3", new OrganizationBaseDTO() { Name = "Organization 3" }, NamingStrategy));
            return this;
        }
    }

}
