using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game.Models.DTO.Game
{
    public class AnswerBaseDTO
    {
        [JsonRequired]
        public Guid UserID { get; set; }

        [JsonRequired]
        public Guid GameId { get; set; }

        [JsonRequired]
        public string Answer { get; set; }
        
    }
}
