using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Abstractions;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Resolvers;
using Newtonsoft.Json.Serialization;

namespace CardDeck.Model.DTO
{
    public class DeckDTO : DeckBaseWithIDDTO
    {
        public List<Guid> CardIDs { get; set; }
        public DeckDTO()
        {
            DeckID = Guid.NewGuid();
        }
    }

    public class DeckDTOExampleGenerator : OpenApiExample<DeckDTO>
    {
        public override IOpenApiExample<DeckDTO> Build(NamingStrategy NamingStrategy = null)
        {
            Examples.Add(OpenApiExampleResolver.Resolve("Deck1", new DeckDTO() { Name = "Deck1", CardIDs = new List<Guid>() { Guid.NewGuid(), Guid.NewGuid() } }, NamingStrategy));
            Examples.Add(OpenApiExampleResolver.Resolve("Deck2", new DeckDTO() { Name = "Deck2", CardIDs = new List<Guid>() { Guid.NewGuid(), Guid.NewGuid() } }, NamingStrategy));
            Examples.Add(OpenApiExampleResolver.Resolve("Deck3", new DeckDTO() { Name = "Deck3", CardIDs = new List<Guid>() { Guid.NewGuid(), Guid.NewGuid() } }, NamingStrategy));
            return this;
        }
    }
}
