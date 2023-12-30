using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Abstractions;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Resolvers;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace UserTeamOrg.Model.DTO
{
    public class EmailDTO
    {
        [JsonRequired]
        public string Email { get; set; }
        [JsonRequired]
        public Guid TeamId { get; set; }
    }

    public class EmailDTOExampleGenerator : OpenApiExample<EmailDTO>
    {
        public override IOpenApiExample<EmailDTO> Build(NamingStrategy namingStrategy = null)
        {
            Examples.Add(OpenApiExampleResolver.Resolve("User 1", new EmailDTO() { Email = "testuser1@email.com", TeamId = Guid.NewGuid() }, namingStrategy));
            Examples.Add(OpenApiExampleResolver.Resolve("User 2", new EmailDTO() { Email = "testuser2@email.com", TeamId = Guid.NewGuid() }, namingStrategy));
            Examples.Add(OpenApiExampleResolver.Resolve("User 3", new EmailDTO() { Email = "testuser3@email.com", TeamId = Guid.NewGuid() }, namingStrategy));
            return this;
        }
    }
}
