using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Abstractions;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Resolvers;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace CardDeck.Model.DTO
{
    public class DeckBaseDTO
    {
        [JsonRequired]
        public string? Name { get; set; }

        [JsonRequired]
        public Guid teamID { get; set; }
    }

    public class DeckBaseDTOExampleGenerator : OpenApiExample<DeckBaseDTO>
    {
        public override IOpenApiExample<DeckBaseDTO> Build(NamingStrategy NamingStrategy)
        {
            Examples.Add(OpenApiExampleResolver.Resolve("DeckBase1", new DeckBaseDTO() { Name = "DeckBase1", teamID = Guid.NewGuid() }, NamingStrategy));
            Examples.Add(OpenApiExampleResolver.Resolve("DeckBase2", new DeckBaseDTO() { Name = "DeckBase2", teamID = Guid.NewGuid() }, NamingStrategy));
            Examples.Add(OpenApiExampleResolver.Resolve("DeckBase3", new DeckBaseDTO() { Name = "DeckBase3", teamID = Guid.NewGuid() }, NamingStrategy));
            return this;
        }
    }
}
