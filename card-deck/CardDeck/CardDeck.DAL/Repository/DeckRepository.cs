using CardDeck.DAL.RepositoryInterface;
using CardDeck.Exceptions.Exceptions;
using CardDeck.Model.DTO;
using CardDeck.Model.Entity;
using Microsoft.EntityFrameworkCore;

namespace CardDeck.DAL.Repository
{
    public class DeckRepository : IDeckRepository
    {

        private readonly CardContext _cardContext;
        public DeckRepository(CardContext cardContext)
        {
            _cardContext = cardContext;
        }

        public async Task CreateDeckAsync(Deck newDeck, Guid teamID)
        {
            //adding the new deck to the Deck table 
            await _cardContext.Decks.AddAsync(newDeck);

            //Saving the changes to the database
            await _cardContext.SaveChangesAsync();

            //Creating a new teamDeck entity to keep track of which team the new deck belongs to
            TeamDeck teamDeck = new()
            {
                DeckID = newDeck.DeckID,
                TeamID = teamID
            };
            //Adding the teamDeck entity to the corresponding table
            await _cardContext.TeamDecks.AddAsync(teamDeck);
            //Saving the changes to the database
            await _cardContext.SaveChangesAsync();
        }

        public async Task DeleteDeckFromTeamAsync(DeckWithDeckIDAndTeamIDDTO deck)
        {
            //finding the to be deleted deck in the database
            Deck deckToDelete = await _cardContext.Decks
                //if DeletedAt value is not null, it means that this deck was deleted.
                .Where(d => d.DeckID == deck.DeckID && d.DeletedAt == null)
                .FirstOrDefaultAsync() ?? throw new EntityNotFoundException("Could not find a deck with this ID");
            //updating the DeletedAt property to the current time to keep track of when the record was deleted, since we never want to delete a record for data analysis
            deckToDelete.DeletedAt = DateTime.Now;

            //saving the changes.
            await _cardContext.SaveChangesAsync();

            //finding the corresponding teamDeck entry of the deck that was deleted
            TeamDeck teamDeckToDelete = await _cardContext.TeamDecks
                .Where(t => t.DeckID == deck.DeckID && t.TeamID == deck.TeamID && t.DeletedAt == null)
                .FirstOrDefaultAsync() ?? throw new EntityNotFoundException("This deck was not part of this team");
            //updating the DeletedAt property to the current time to keep track of when the record was deleted, since we never want to delete a record for data analysis
            teamDeckToDelete.DeletedAt = DateTime.Now;

            //saving the changes to the database
            await _cardContext.SaveChangesAsync();
        }

        public async Task<List<Deck>> GetAllDecksAsync(Guid teamID)
        {
            //finding all deckIDs of the decks that belong to the team that was given.
            List<Guid> ids = await _cardContext.TeamDecks
                .Where(t => t.TeamID == teamID && t.DeletedAt == null)
                .Select(t => t.DeckID)
                .ToListAsync() ?? throw new EntityNotFoundException("There are no decks in the given team");

            //going through the list of IDs, adding the decks with these deckIDs to a list which is then returned, including the child objects CardDeck & TeamDeck
            List<Deck> decks = await _cardContext.Decks
                .Where(d => ids.Contains(d.DeckID))
                .ToListAsync();

            return decks;
        }

        public async Task<Deck> GetDeckAsync(Guid deckID, Guid teamID)
        {
            Deck deck = new();
            //if the teamdeck object exists, the given deckid and teamid are a valid combination
            TeamDeck teamDeck = await _cardContext.TeamDecks
                .Where(d => d.DeckID == deckID && d.TeamID == teamID && d.DeletedAt == null)
                .FirstOrDefaultAsync() ?? throw new EntityNotFoundException("Could not find the deck in the given team");
            //receiving and the returning the deck, including the CardDeck entry to see what deck the cards belong to.
            deck = await _cardContext.Decks
                .Where(d => d.DeckID == deckID && d.DeletedAt == null)
                .Include(d => d.CardDecks)
                .FirstOrDefaultAsync() ?? throw new EntityNotFoundException("Could not find a Deck with this ID");

            return deck;
        }

        public async Task UpdateDeckAsync(DeckBaseWithIDDTO deck)
        {
            //finding the deck that should be updated.
            Deck deckToUpdate = await _cardContext.Decks
                .Where(d => d.DeckID == deck.DeckID && d.DeletedAt == null)
                .FirstOrDefaultAsync() ?? throw new EntityNotFoundException("Could not find a Deck with this ID");
            //setting the values of the updated deck to the to be updated deck
            _cardContext.Entry(deckToUpdate).CurrentValues.SetValues(deck);

            //saving the changes to the database
            await _cardContext.SaveChangesAsync();
        }
    }
}
