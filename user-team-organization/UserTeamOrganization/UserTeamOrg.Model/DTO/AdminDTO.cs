using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Abstractions;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Resolvers;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserTeamOrg.Model.Entity;

namespace UserTeamOrg.Model.DTO
{
    public class AdminDTO : PersonDTO
    {
        [JsonRequired]
        public Guid AdminId { get; set; }

        [JsonRequired]
        public AccessLevel AccessLevel { get; set; }

    }

    public class AdminDTOExampleGenerator : OpenApiExample<AdminDTO>
    {
        public override IOpenApiExample<AdminDTO> Build(NamingStrategy namingStrategy = null)
        {
            Examples.Add(OpenApiExampleResolver.Resolve("User 1", new AdminDTO() { FirstName = "Admin1", LastName = "van User1", Email = "testuser1@email.com", Password = "Secret", AccessLevel = AccessLevel.SuperAdmin, ImageUrl = null, DeletedAt = null }, namingStrategy));
            Examples.Add(OpenApiExampleResolver.Resolve("User 2", new AdminDTO() { FirstName = "Admin2", LastName = "van User2", Email = "testuser2@email.com", Password = "Secret", AccessLevel = AccessLevel.Admin, ImageUrl = null, DeletedAt = null }, namingStrategy));
            Examples.Add(OpenApiExampleResolver.Resolve("User 3", new AdminDTO() { FirstName = "Admin3", LastName = "van User3", Email = "testuser3@email.com", Password = "Secret", AccessLevel = AccessLevel.TeamLeader, ImageUrl = null, DeletedAt = null }, namingStrategy));
            return this;
        }
    }
}

