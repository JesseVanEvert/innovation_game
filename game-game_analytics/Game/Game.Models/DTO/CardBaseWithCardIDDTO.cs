using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Abstractions;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Resolvers;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Game.Models.DTO
{
    public class CardBaseWithCardIDDTO : CardBaseDTO
    {
        [JsonRequired]
        public Guid CardID { get; set; }
    }

    public class CardBaseWithCardIDDTOExampleGenerator : OpenApiExample<CardBaseWithCardIDDTO>
    {
        public override IOpenApiExample<CardBaseWithCardIDDTO> Build(NamingStrategy NamingStrategy)
        {
            Examples.Add(OpenApiExampleResolver.Resolve("DeckWithID1", new CardBaseWithCardIDDTO() { FrontSideText = "FrontText1", BackSideText = "BackSide1", ImageUrl = "imageurl1", CardID = Guid.NewGuid() }, NamingStrategy));
            Examples.Add(OpenApiExampleResolver.Resolve("DeckWithID2", new CardBaseWithCardIDDTO() { FrontSideText = "FrontText2", BackSideText = "BackSide2", ImageUrl = "imageurl2", CardID = Guid.NewGuid() }, NamingStrategy));
            Examples.Add(OpenApiExampleResolver.Resolve("DeckWithID3", new CardBaseWithCardIDDTO() { FrontSideText = "FrontText3", BackSideText = "BackSide3", ImageUrl = "imageurl3", CardID = Guid.NewGuid() }, NamingStrategy));
            return this;
        }
    }
}
