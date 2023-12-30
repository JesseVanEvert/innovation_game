using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Abstractions;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Resolvers;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace CardDeck.Model.DTO
{
    public class DeckBaseWithIDDTO : DeckBaseDTO
    {
        [JsonRequired]
        public Guid DeckID { get; set; }
    }

    public class DeckBaseWithIDDTOExampleGenerator : OpenApiExample<DeckBaseWithIDDTO>
    {
        public override IOpenApiExample<DeckBaseWithIDDTO> Build(NamingStrategy NamingStrategy)
        {
            Examples.Add(OpenApiExampleResolver.Resolve("DeckBaseWithID1", new DeckBaseWithIDDTO() { Name = "Deck1", DeckID = Guid.NewGuid() }, NamingStrategy));
            Examples.Add(OpenApiExampleResolver.Resolve("DeckBase2WithID", new DeckBaseWithIDDTO() { Name = "Deck2",  DeckID = Guid.NewGuid() }, NamingStrategy));
            Examples.Add(OpenApiExampleResolver.Resolve("DeckBaseWithID3", new DeckBaseWithIDDTO() { Name = "Deck3",  DeckID = Guid.NewGuid() }, NamingStrategy));
            return this;
        }
    }
}
