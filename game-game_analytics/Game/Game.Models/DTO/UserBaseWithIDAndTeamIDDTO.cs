using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Abstractions;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Resolvers;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game.Models.DTO
{
    public class UserBaseWithIDAndTeamIDDTO : UserBaseWithTeamIDDTO
    {
        public Guid UserId { get; set; }
    }

    public class PersonBaseWithIDAndTeamIDDTOExampleGenerator : OpenApiExample<UserBaseWithIDAndTeamIDDTO>
    {
        public override IOpenApiExample<UserBaseWithIDAndTeamIDDTO> Build(NamingStrategy namingStrategy = null)
        {
            Examples.Add(OpenApiExampleResolver.Resolve("Person 1", new UserBaseWithIDAndTeamIDDTO() { TeamId = Guid.NewGuid(), UserId = Guid.NewGuid(), FirstName = "Test1", LastName = "van User1", Email = "testuser1@email.com", Password = "Secret" }, namingStrategy));
            Examples.Add(OpenApiExampleResolver.Resolve("Person 2", new UserBaseWithIDAndTeamIDDTO() { TeamId = Guid.NewGuid(),  UserId = Guid.NewGuid(), FirstName = "Test2", LastName = "van User2", Email = "testuser2@email.com", Password = "Secret" }, namingStrategy));
            Examples.Add(OpenApiExampleResolver.Resolve("Person 3", new UserBaseWithIDAndTeamIDDTO() { TeamId = Guid.NewGuid(),  UserId = Guid.NewGuid(), FirstName = "Test3", LastName = "van User3", Email = "testuser3@email.com", Password = "Secret" }, namingStrategy));
            return this;
        }
    }
}
