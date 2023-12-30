using CardDeck.Model.Entity;
using Microsoft.EntityFrameworkCore;

namespace CardDeck.DAL
{
    public class CardContext : DbContext
    {
        public DbSet<Card> Cards { get; set; }
        public DbSet<Model.Entity.CardDeck> CardDecks { get; set; }
        public DbSet<Deck> Decks { get; set; }
        public DbSet<TeamDeck> TeamDecks { get; set; }

        public CardContext(DbContextOptions<CardContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Model.Entity.CardDeck>()
                .HasKey(gu => new { gu.CardID, gu.DeckID });
            modelBuilder.Entity<TeamDeck>()
                .HasKey(gu => new { gu.DeckID, gu.TeamID });
        }
    }
}
