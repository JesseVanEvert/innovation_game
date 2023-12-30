using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game.Models.DTO.Game
{
    public class ScoreDTO
    {
        //Player who gives the scores
        public Guid UserID { get; set; }
        public Guid GameID { get; set; }
        public Guid AnswerID { get; set; }
        public int Score { get; set; }
    }
}
