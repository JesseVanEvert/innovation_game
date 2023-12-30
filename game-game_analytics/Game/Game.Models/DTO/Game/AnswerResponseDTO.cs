using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game.Models.DTO.Game
{
    public class AnswerResponseDTO
    {
        public string PlayerName { get; set; }
        public string Answer { get; set; }
        public int Score { get; set; }
    }
}
