using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Abstractions;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Resolvers;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserTeamOrg.Model.DTO
{
    public class AdminBaseWithIDDTO : PersonBaseWithIDDTO
    {
        [JsonRequired]
        public Guid AdminId { get; set; }
    }

    public class AdminBaseWithIDDTOExamplaeGenerator : OpenApiExample<AdminBaseWithIDDTO>
    {
        public override IOpenApiExample<AdminBaseWithIDDTO> Build(NamingStrategy namingStrategy = null)
        {
            Examples.Add(OpenApiExampleResolver.Resolve("Admin 1", new AdminBaseWithIDDTO() { AdminId = Guid.NewGuid(), PersonId = Guid.NewGuid(), FirstName = "Test1", LastName = "van User1", Email = "testuser1@email.com", Password = "Secret" }, namingStrategy));
            Examples.Add(OpenApiExampleResolver.Resolve("Admin 2", new AdminBaseWithIDDTO() { AdminId = Guid.NewGuid(), PersonId = Guid.NewGuid(), FirstName = "Test2", LastName = "van User2", Email = "testuser2@email.com", Password = "Secret" }, namingStrategy));
            Examples.Add(OpenApiExampleResolver.Resolve("Admin 3", new AdminBaseWithIDDTO() { AdminId = Guid.NewGuid(), PersonId = Guid.NewGuid(), FirstName = "Test3", LastName = "van User3", Email = "testuser3@email.com", Password = "Secret" }, namingStrategy));
            return this;
        }
    }
}

