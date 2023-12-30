using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Abstractions;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Resolvers;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game.Models.DTO
{
    public class GameUserBodyDTO
    {
        //This DTO is used as the request body for the JoinGame endpoint
        [JsonRequired]
        public Guid UserID { get; set; }
        [JsonRequired]
        public Guid GameID { get; set; }
    }

    /*public class GameUserBodyExampleDTOGenerator : OpenApiExample<GameUserBodyDTO>
    {
        public override IOpenApiExample<GameUserBodyDTO> Build(NamingStrategy NamingStrategy = null)
        {
            Examples.Add(OpenApiExampleResolver.Resolve("Instance1", new GameUserBodyDTO() { UserID = Guid.NewGuid(), GameID = Guid.NewGuid() }, NamingStrategy));
            Examples.Add(OpenApiExampleResolver.Resolve("Instance2", new GameUserBodyDTO() { UserID = Guid.NewGuid(), GameID = Guid.NewGuid() }, NamingStrategy));
            Examples.Add(OpenApiExampleResolver.Resolve("Instance3", new GameUserBodyDTO() { UserID = Guid.NewGuid(), GameID = Guid.NewGuid() }, NamingStrategy));
            return this;
        }
    }*/
}
