using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Abstractions;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Resolvers;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserTeamOrg.Model.DTO
{
    public class UserDTO : PersonDTO
    {
        [JsonRequired]
        public Guid UserId { get; set; }
    }

    public class UserDTOExampleGenerator : OpenApiExample<UserDTO>
    {
        public override IOpenApiExample<UserDTO> Build(NamingStrategy namingStrategy = null)
        {
            Examples.Add(OpenApiExampleResolver.Resolve("User1", new UserDTO() {  UserId = Guid.NewGuid(), FirstName = "Test 1", LastName = "Last name 1", Email = "Test1@email.com", Password = "Secret", ImageUrl = null, DeletedAt = null }, namingStrategy));
            Examples.Add(OpenApiExampleResolver.Resolve("User2", new UserDTO() {  UserId = Guid.NewGuid(), FirstName = "Test 2", LastName = "Last name 2", Email = "Test2@email.com", Password = "Secret", ImageUrl = null, DeletedAt = null }, namingStrategy));
            Examples.Add(OpenApiExampleResolver.Resolve("User3", new UserDTO() {  UserId = Guid.NewGuid(), FirstName = "Test 3", LastName = "Last name 3", Email = "Test3@email.com", Password = "Secret", ImageUrl = null, DeletedAt = null }, namingStrategy));
            return this;
        }
    }
}

