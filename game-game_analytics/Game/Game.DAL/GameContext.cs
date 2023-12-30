using Game.Models.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Game.DAL
{
    public class GameContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Models.Entity.Game> Games { get; set; }
        public DbSet<GameUser> GameUsers { get; set; }
        public DbSet<GameUserAnswer> GameUsersAnswers { get; set; }
        public DbSet<GameUserScore> GameUsersScores { get; set; }
        public DbSet<Card> Cards { get; set; }
        public DbSet<UserTeam> UserTeams { get; set; }  
        public DbSet<Team> Teams { get; set; }
        public DbSet<Organization> Organizations { get; set; }  
        public DbSet<Deck> Decks { get; set; }
        public DbSet<TeamDeck> TeamDecks { get; set; }
        public DbSet<CardDeck> CardDecks { get; set; }

        public GameContext(DbContextOptions<GameContext> options) : base(options){
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<GameUser>()
                .HasKey(gu => new { gu.UserID, gu.GameID });

            modelBuilder.Entity<GameUserAnswer>()
                .HasKey(gua => gua.GameUserAnswerID);

            modelBuilder.Entity<GameUserScore>()
                .HasKey(gus => new { gus.GameUserAnswerID, gus.CardID });

            modelBuilder.Entity<UserTeam>()
                .HasKey(ut => new {ut.UserID, ut.TeamID });

            modelBuilder.Entity<CardDeck>()
                .HasKey(cd => new { cd.CardID, cd.DeckID });

            modelBuilder.Entity<TeamDeck>()
                .HasKey(td => new { td.TeamID, td.DeckID });
        }
    }
}