using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Abstractions;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Resolvers;


namespace UserTeamOrg.Model.DTO
{
    public class UserBaseWithTeamIDDTO : PersonBaseDTO
    {
        [JsonRequired]
        public Guid TeamId { get; set; }
    }

    public class UserBaseWithTeamIDDTOExampleGenerator : OpenApiExample<UserBaseWithTeamIDDTO>
    {
        public override IOpenApiExample<UserBaseWithTeamIDDTO> Build(NamingStrategy namingStrategy = null)
        {
            Examples.Add(OpenApiExampleResolver.Resolve("User 1", new UserBaseWithTeamIDDTO() { TeamId = Guid.NewGuid(), FirstName = "Test1", LastName = "van User1", Email = "testuser1@email.com", Password = "Secret" }, namingStrategy));
            Examples.Add(OpenApiExampleResolver.Resolve("User 2", new UserBaseWithTeamIDDTO() { TeamId = Guid.NewGuid(), FirstName = "Test2", LastName = "van User2", Email = "testuser2@email.com", Password = "Secret" }, namingStrategy));
            Examples.Add(OpenApiExampleResolver.Resolve("User 3", new UserBaseWithTeamIDDTO() { TeamId = Guid.NewGuid(), FirstName = "Test3", LastName = "van User3", Email = "testuser3@email.com", Password = "Secret" }, namingStrategy));
            return this;
        }
    }

}

