using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game.Models.Entity
{
    public class GameUserScore
    {
        [Key, Column(Order = 0)]
        public Guid GameUserAnswerID { get; set; }
        [Key, Column(Order = 1)]
        public Guid CardID { get; set; }

        public virtual GameUserAnswer GameUserAnswer { get; set; }
        public virtual Card Card { get; set; }

        public int Score { get; set; }
    }
}
