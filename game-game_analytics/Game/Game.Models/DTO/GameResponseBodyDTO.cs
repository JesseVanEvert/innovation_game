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
    public class GameResponseBodyDTO
    {
        [JsonRequired]
        public Guid GameID { get; set; }

        //The code users can use to join a game, distributed by the game admin
        /*[JsonRequired]
        public int GameCode { get; set; }*/

        //the startdate and enddate of the game will help analyze ideas later on
        [JsonRequired]
        public DateTime GameStart { get; set; }

        /*[JsonRequired]
        public DateTime GameEndDate { get; set; }*/

        //The teamid of the team the game belongs to.
        /*[JsonRequired]
        public Guid TeamID { get; set; }*/
    }

   /* public class GameDTOExampleGenerator : OpenApiExample<GameResponseBodyDTO>
    {
        public override IOpenApiExample<GameResponseBodyDTO> Build(NamingStrategy NamingStrategy = null)
        {
            Examples.Add(OpenApiExampleResolver.Resolve("Game1", new GameResponseBodyDTO() { GameID = Guid.NewGuid(), GameCode = 1234, GameStartDate = DateTime.Now, GameEndDate = DateTime.MaxValue, TeamID = Guid.NewGuid() }, NamingStrategy));
            Examples.Add(OpenApiExampleResolver.Resolve("Game2", new GameResponseBodyDTO() { GameID = Guid.NewGuid(), GameCode = 1235, GameStartDate = DateTime.Now, GameEndDate = DateTime.MaxValue, TeamID = Guid.NewGuid() }, NamingStrategy));
            Examples.Add(OpenApiExampleResolver.Resolve("Game3", new GameResponseBodyDTO() { GameID = Guid.NewGuid(), GameCode = 1236, GameStartDate = DateTime.Now, GameEndDate = DateTime.MaxValue, TeamID = Guid.NewGuid() }, NamingStrategy));
            return this;
        }
    }*/
}
