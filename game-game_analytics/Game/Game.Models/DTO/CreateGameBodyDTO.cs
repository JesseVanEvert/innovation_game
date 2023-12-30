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
    public class CreateGameBodyDTO
    {
        [JsonRequired]
        public string Name { get; set; }
        [JsonRequired]
        public Guid DeckID { get; set; }
    }

    /*public class CreateGameBodyDTOExampleGenerator : OpenApiExample<CreateGameBodyDTO>
    {
        public override IOpenApiExample<CreateGameBodyDTO> Build(NamingStrategy NamingStrategy = null)
        {
            Examples.Add(OpenApiExampleResolver.Resolve("Game1", new CreateGameBodyDTO() { Name = "Discussion new coffee machine" }, NamingStrategy));
            Examples.Add(OpenApiExampleResolver.Resolve("Game2", new CreateGameBodyDTO() { Name = "How to improve elderly care?" }, NamingStrategy));
            Examples.Add(OpenApiExampleResolver.Resolve("Game3", new CreateGameBodyDTO() { Name = "Brainstorm" }, NamingStrategy));
            return this;
        }
    }*/
}
