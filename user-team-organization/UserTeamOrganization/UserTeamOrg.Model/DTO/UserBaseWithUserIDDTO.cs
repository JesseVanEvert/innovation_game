using System;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Abstractions;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Resolvers;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace UserTeamOrg.Model.DTO
{
    public class UserBaseWithUserIDDTO : PersonBaseWithIDDTO
    {
        [JsonRequired]
        public Guid UserId { get; set; }
    }

    public class UserBaseWithUserIDDTOExampleGenerator : OpenApiExample<UserBaseWithUserIDDTO>
    {
        public override IOpenApiExample<UserBaseWithUserIDDTO> Build(NamingStrategy namingStrategy = null)
        {
            Examples.Add(OpenApiExampleResolver.Resolve("User 1", new UserBaseWithUserIDDTO() { UserId = Guid.NewGuid(), PersonId = Guid.NewGuid() ,FirstName = "Test1", LastName = "van User1", Email = "testuser1@email.com", Password = "Secret" }, namingStrategy));
            Examples.Add(OpenApiExampleResolver.Resolve("User 2", new UserBaseWithUserIDDTO() { UserId = Guid.NewGuid(), PersonId = Guid.NewGuid(), FirstName = "Test2", LastName = "van User2", Email = "testuser2@email.com", Password = "Secret" }, namingStrategy));
            Examples.Add(OpenApiExampleResolver.Resolve("User 3", new UserBaseWithUserIDDTO() { UserId = Guid.NewGuid(), PersonId = Guid.NewGuid(), FirstName = "Test3", LastName = "van User3", Email = "testuser3@email.com", Password = "Secret" }, namingStrategy));
            return this;
        }
    }
}

