using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Abstractions;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Resolvers;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Game.Models.DTO
{
    public class CardBaseWithDeckIDDTO : CardBaseDTO
    {
        [JsonRequired]
        public Guid DeckID { get; set; }
    }

    public class CardBaseWithDeckIDDTOExampleGenerator : OpenApiExample<CardBaseWithDeckIDDTO>
    {
        public override IOpenApiExample<CardBaseWithDeckIDDTO> Build(NamingStrategy NamingStrategy)
        {
            Examples.Add(OpenApiExampleResolver.Resolve("DeckWithID1", new CardBaseWithDeckIDDTO() { FrontSideText = "voorkant1", BackSideText = "achterkant1", ImageUrl = "imageurl1", DeckID = Guid.NewGuid() }, NamingStrategy));
            Examples.Add(OpenApiExampleResolver.Resolve("DeckWithID2", new CardBaseWithDeckIDDTO() { FrontSideText = "voorkant2", BackSideText = "achterkant2", ImageUrl = "imageurl2", DeckID = Guid.NewGuid() }, NamingStrategy));
            Examples.Add(OpenApiExampleResolver.Resolve("DeckWithID3", new CardBaseWithDeckIDDTO() { FrontSideText = "voorkant3", BackSideText = "achterkant3", ImageUrl = "imageurl3", DeckID = Guid.NewGuid() }, NamingStrategy));
            return this;
        }
    }
}

