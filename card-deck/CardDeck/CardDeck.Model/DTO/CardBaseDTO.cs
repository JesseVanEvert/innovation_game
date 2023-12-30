using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Abstractions;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Resolvers;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace CardDeck.Model.DTO
{
    public class CardBaseDTO
    {
        [JsonRequired]
        public string? FrontSideText { get; set; }

        [JsonRequired]
        public string? BackSideText { get; set; }

        public string? ImageUrl { get; set; }
    }

    public class CardBaseDTOExampleGenerator : OpenApiExample<CardBaseDTO>
    {
        public override IOpenApiExample<CardBaseDTO> Build(NamingStrategy NamingStrategy)
        {
            Examples.Add(OpenApiExampleResolver.Resolve("Card1", new CardBaseDTO() { FrontSideText = "voorkant1", BackSideText = "achterkant1", ImageUrl = "imageurl1" }, NamingStrategy));
            Examples.Add(OpenApiExampleResolver.Resolve("Card2", new CardBaseDTO() { FrontSideText = "voorkant2", BackSideText = "achterkant2", ImageUrl = "imageurl2" }, NamingStrategy));
            Examples.Add(OpenApiExampleResolver.Resolve("Card3", new CardBaseDTO() { FrontSideText = "voorkant3", BackSideText = "achterkant3", ImageUrl = "imageurl3" }, NamingStrategy));
            return this;
        }
    }
}
