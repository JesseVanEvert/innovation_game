
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Game.Models.Entity
{
    public class GameUserAnswer
    {
        public GameUserAnswer()
        {
            GameUserScores = new HashSet<GameUserScore>();
        }

        [Key, Column(Order = 0)]
        public Guid GameUserAnswerID { get; set; }
        [Key, Column(Order = 1)]
        public Guid UserID { get; set; }

        public GameUser GameUser;

        public string Answer { get; set; }

        public virtual ICollection<GameUserScore>? GameUserScores { get; set; }
    }
}
