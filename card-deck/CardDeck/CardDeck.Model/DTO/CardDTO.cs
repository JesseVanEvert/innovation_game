using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Abstractions;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Resolvers;
using Newtonsoft.Json.Serialization;

namespace CardDeck.Model.DTO
{
    public class CardDTO : CardBaseWithCardIDDTO
    {

        public CardDTO()
        {
            CardID = Guid.NewGuid();
        }
    }
    public class CardDTOExampleGenerator : OpenApiExample<CardDTO>
    {
        public override IOpenApiExample<CardDTO> Build(NamingStrategy NamingStrategy)
        {
            Examples.Add(OpenApiExampleResolver.Resolve("Card1", new CardDTO() { FrontSideText = "voorkant1", BackSideText = "achterkant1", ImageUrl = "imageurl1" }, NamingStrategy));
            Examples.Add(OpenApiExampleResolver.Resolve("Card2", new CardDTO() { FrontSideText = "voorkant2", BackSideText = "achterkant2", ImageUrl = "imageurl2" }, NamingStrategy));
            Examples.Add(OpenApiExampleResolver.Resolve("Card3", new CardDTO() { FrontSideText = "voorkant3", BackSideText = "achterkant3", ImageUrl = "imageurl3" }, NamingStrategy));
            return this;
        }
    }
}
