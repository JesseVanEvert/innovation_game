using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Abstractions;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Resolvers;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace CardDeck.Model.DTO
{
    public class DeckWithIDDTO
    {
        [JsonRequired]
        public Guid DeckID { get; set; }
    }

    public class DeckWithIDDTOExampleGenerator : OpenApiExample<DeckWithIDDTO>
    {
        public override IOpenApiExample<DeckWithIDDTO> Build(NamingStrategy NamingStrategy)
        {
            Examples.Add(OpenApiExampleResolver.Resolve("DeckWithID1", new DeckWithIDDTO() { DeckID = Guid.NewGuid() }, NamingStrategy));
            Examples.Add(OpenApiExampleResolver.Resolve("DeckWithID2", new DeckWithIDDTO() { DeckID = Guid.NewGuid() }, NamingStrategy));
            Examples.Add(OpenApiExampleResolver.Resolve("DeckWithID3", new DeckWithIDDTO() { DeckID = Guid.NewGuid() }, NamingStrategy));
            return this;
        }
    }
}
