using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Abstractions;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Resolvers;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game.Models.DTO
{
    public class PersonDTO : PersonBaseWithIDDTO
    {        
        public string? ImageUrl { get; set; }
        
        public DateTime? DeletedAt { get; set; }
    }

    public class PersonDTOExampleGenerator : OpenApiExample<PersonDTO>
    {
        public override IOpenApiExample<PersonDTO> Build(NamingStrategy namingStrategy = null)
        {
            Examples.Add(OpenApiExampleResolver.Resolve("User 1", new PersonDTO() { FirstName = "Test1", LastName = "van User1", Email = "testuser1@email.com", Password = "Secret", ImageUrl = null, DeletedAt = null }, namingStrategy));
            Examples.Add(OpenApiExampleResolver.Resolve("User 2", new PersonDTO() { FirstName = "Test2", LastName = "van User2", Email = "testuser2@email.com", Password = "Secret", ImageUrl = null, DeletedAt = null }, namingStrategy));
            Examples.Add(OpenApiExampleResolver.Resolve("User 3", new PersonDTO() { FirstName = "Test3", LastName = "van User3", Email = "testuser3@email.com", Password = "Secret", ImageUrl = null, DeletedAt = null }, namingStrategy));
            return this;
        }
    }
}

