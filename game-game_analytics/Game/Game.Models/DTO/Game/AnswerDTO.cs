using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Abstractions;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Resolvers;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game.Models.DTO.Game
{
    public class AnswerDTO : AnswerBaseDTO
    {
        public Guid AnswerID { get; set; }
    }

    /*public class AnswerDTOExample : OpenApiExample<AnswerDTO>
    {
        public override IOpenApiExample<AnswerDTO> Build(NamingStrategy NamingStrategy = null)
        {
            Examples.Add(OpenApiExampleResolver.Resolve("Instance1", new AnswerDTO() { UserID = Guid.NewGuid(), GameId = Guid.NewGuid(), CardId = Guid.NewGuid(), Answer = "An answer" }, NamingStrategy));
            Examples.Add(OpenApiExampleResolver.Resolve("Instance2", new AnswerDTO() { UserID = Guid.NewGuid(), GameId = Guid.NewGuid(), CardId = Guid.NewGuid(), Answer = "Second answer" }, NamingStrategy));
            Examples.Add(OpenApiExampleResolver.Resolve("Instance3", new AnswerDTO() { UserID = Guid.NewGuid(), GameId = Guid.NewGuid(), CardId = Guid.NewGuid(), Answer = "Third answer" }, NamingStrategy));
            return this;
        }
    }*/
}
