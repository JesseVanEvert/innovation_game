using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Abstractions;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Resolvers;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserTeamOrg.Model.DTO
{
    public class PersonBaseDTO
    {
        [JsonRequired]
        public string FirstName { get; set; }
        [JsonRequired]
        public string LastName { get; set; }
        [JsonRequired]
        public string Email { get; set; }
        public string Password { get; set; }
    }

    public class PersonBaseDTOExampleGenerator : OpenApiExample<PersonBaseDTO>
    {
        public override IOpenApiExample<PersonBaseDTO> Build(NamingStrategy namingStrategy = null)
        {
            Examples.Add(OpenApiExampleResolver.Resolve("Person 1", new PersonBaseDTO() { FirstName = "Test1", LastName = "van User1", Email = "testuser1@email.com", Password = "Secret" }, namingStrategy));
            Examples.Add(OpenApiExampleResolver.Resolve("Person 2", new PersonBaseDTO() { FirstName = "Test2", LastName = "van User2", Email = "testuser2@email.com", Password = "Secret" }, namingStrategy));
            Examples.Add(OpenApiExampleResolver.Resolve("Person 3", new PersonBaseDTO() { FirstName = "Test3", LastName = "van User3", Email = "testuser3@email.com", Password = "Secret" }, namingStrategy));
            return this;
        }
    }
}

