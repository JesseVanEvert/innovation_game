using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Abstractions;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Resolvers;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Game.Models.DTO
{
    public class CardWithCardIDAndDeckIDDTO
    {
        [JsonRequired]
        public Guid DeckID { get; set; }

        [JsonRequired]
        public Guid CardID { get; set; }
    }

    public class CardWithCardIDAndDeckIDDTOExampleGenerator : OpenApiExample<CardWithCardIDAndDeckIDDTO>
    {
        public override IOpenApiExample<CardWithCardIDAndDeckIDDTO> Build(NamingStrategy NamingStrategy)
        {
            Examples.Add(OpenApiExampleResolver.Resolve("DeckWithID1", new CardWithCardIDAndDeckIDDTO() { DeckID = Guid.NewGuid(), CardID = Guid.NewGuid() }, NamingStrategy));
            Examples.Add(OpenApiExampleResolver.Resolve("DeckWithID2", new CardWithCardIDAndDeckIDDTO() { DeckID = Guid.NewGuid(), CardID = Guid.NewGuid() }, NamingStrategy));
            Examples.Add(OpenApiExampleResolver.Resolve("DeckWithID3", new CardWithCardIDAndDeckIDDTO() { DeckID = Guid.NewGuid(), CardID = Guid.NewGuid() }, NamingStrategy));
            return this;
        }
    }
}
