using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Abstractions;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Resolvers;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Game.Models.DTO
{
    public class DeckWithDeckIDAndTeamIDDTO :DeckWithIDDTO
    {
        [JsonRequired]
        public Guid TeamID { get; set;}
    }

    public class DeckBaseWithDeckIDAndTeamIDDTOExampleGenerator : OpenApiExample<DeckBaseWithDeckIDAndTeamIDDTOExampleGenerator>
    {
        public override IOpenApiExample<DeckBaseWithDeckIDAndTeamIDDTOExampleGenerator> Build(NamingStrategy NamingStrategy)
        {
            Examples.Add(OpenApiExampleResolver.Resolve("DeckBaseWithID1", new DeckWithDeckIDAndTeamIDDTO() { DeckID = Guid.NewGuid(), TeamID = Guid.NewGuid() }, NamingStrategy));
            Examples.Add(OpenApiExampleResolver.Resolve("DeckBase2WithID", new DeckWithDeckIDAndTeamIDDTO() { DeckID = Guid.NewGuid(), TeamID = Guid.NewGuid() }, NamingStrategy));
            Examples.Add(OpenApiExampleResolver.Resolve("DeckBaseWithID3", new DeckWithDeckIDAndTeamIDDTO() { DeckID = Guid.NewGuid(), TeamID = Guid.NewGuid() }, NamingStrategy));
            return this;
        }
    }
}
