using CardDeck.DAL.RepositoryInterface;
using CardDeck.Exceptions.Exceptions;
using CardDeck.Model.DTO;
using CardDeck.Model.Entity;
using Microsoft.EntityFrameworkCore;

namespace CardDeck.DAL.Repository
{
    public class CardRepository : ICardRepository
    {

        private readonly CardContext _cardContext;
        public CardRepository(CardContext cardContext)
        {
            _cardContext = cardContext;
        }
        public async Task CreateCardInDeckAsync(Card card, Guid deckID)
        {
            Deck deck = await _cardContext.Decks
                .Where(d => d.DeckID == deckID)
                .FirstOrDefaultAsync() ?? throw new EntityNotFoundException("This deckid does not exist");

            //adding the card to the Card table
            await _cardContext.Cards.AddAsync(card);

            //saving the changes to the database
            await _cardContext.SaveChangesAsync();

            //creating the CardDeck entity to keep track of which deck the card belongs to
            Model.Entity.CardDeck cardDeck = new()
            {
                CardID = card.CardID,
                DeckID = deckID,
                DeletedAt = null
            };
            //adding the CardDeck entity to the CardDeck table
            await _cardContext.CardDecks.AddAsync(cardDeck);

            //saving the changes to the database
            await _cardContext.SaveChangesAsync();
        }

        public async Task DeleteCardInDeckAsync(CardWithCardIDAndDeckIDDTO cardBase)
        {
            //finding the Card that has the given CardID
            Card? cardToDelete = await _cardContext.Cards
                .Where(c => c.CardID == cardBase.CardID && c.DeletedAt == null)
                .FirstOrDefaultAsync() ?? throw new EntityNotFoundException("Could not find a card with this ID");
            //updating the DeletedAt property to the current time to keep track of when the record was deleted, since we never want to delete a record for data analysis
            cardToDelete.DeletedAt = DateTime.Now;

            //saving the changes to the database
            await _cardContext.SaveChangesAsync();

            //finding the corresponding CardDeck entity which should be deleted
            Model.Entity.CardDeck? cardDeckToDelete = await _cardContext.CardDecks
                .Where(c => c.CardID == cardBase.CardID && c.DeckID == cardBase.DeckID && c.DeletedAt == null)
                .FirstOrDefaultAsync() ?? throw new EntityNotFoundException("This Card does not exist in the given Deck");
            //updating the DeletedAt property to the current time to keep track of when the record was deleted, since we never want to delete a record for data analysis
            cardDeckToDelete.DeletedAt = DateTime.Now;

            //saving the changes to the database
            await _cardContext.SaveChangesAsync();
        }

        public async Task<Card> GetCardFromDeckAsync(Guid cardID)
        {
            //finding and returning the card that was asked for with the CardID
            return await _cardContext.Cards
                .Where(c => c.CardID == cardID && c.DeletedAt == null)
                .Include(c => c.CardDecks)
                .FirstOrDefaultAsync() ?? throw new EntityNotFoundException("Could not find a card with this ID");
        }

        public async Task UpdateCardInDeckAsync(CardBaseWithCardIDDTO cardBase)
        {
            //finding the card that should be updated.
            Card cardToUpdate = await _cardContext.Cards
                .Where(c => c.CardID == cardBase.CardID && c.DeletedAt == null)
                .FirstOrDefaultAsync() ?? throw new EntityNotFoundException("Could not find a card with this ID");
            //updating the card that should be updated with the new values
            _cardContext.Entry(cardToUpdate).CurrentValues.SetValues(cardBase);

            //saving  the changes to the database
            await _cardContext.SaveChangesAsync();
        }

        //Get all cards from a deck
        /* public async Task<List<Card>> GetAllCardsFromDeckAsync(Guid deckID)
         {
             //finding the deck that the cards should be returned from
             Deck deck = await _cardContext.Decks
                 .Where(d => d.DeckID == deckID)
                 .FirstOrDefaultAsync() ?? throw new EntityNotFoundException("This deckid does not exist");

             //finding all the CardDeck entities that has the given DeckID
             List<Model.Entity.CardDeck> cardDecks = await _cardContext.CardDecks
                 .Where(cd => cd.DeckID == deckID && cd.DeletedAt == null)
                 .ToListAsync() ?? throw new EntityNotFoundException("This deck does not contain any cards");

             //creating a list of cards that should be returned
             List<Card> cards = new();

             //looping through all the CardDeck entities that has the given DeckID
             foreach (Model.Entity.CardDeck cardDeck in cardDecks)
             {
                 //finding the card that has the CardID that is in the CardDeck entity
                 Card card = await _cardContext.Cards
                     .Where(c => c.CardID == cardDeck.CardID && c.DeletedAt == null)
                     .FirstOrDefaultAsync() ?? throw new EntityNotFoundException("Could not find a card with this ID");
                 //adding the card to the list of cards that should be returned
                 cards.Add(card);
             }
             //returning the list of cards
             return cards;
         }*/

        public async Task<List<Card>> GetAllCardsFromDeckAsync(Guid deckID)
        {
            // Find the deck with the given ID
            Deck deck = await _cardContext.Decks
                .FirstOrDefaultAsync(d => d.DeckID == deckID);

            if (deck == null)
            {
                throw new EntityNotFoundException("This deckid does not exist");
            }

            // Find the card IDs associated with the deck
            List<Guid> cardIDs = await _cardContext.CardDecks
                .Where(cd => cd.DeckID == deckID && cd.DeletedAt == null)
                .Select(cd => cd.CardID)
                .ToListAsync();

            if (cardIDs.Count == 0)
            {
                throw new EntityNotFoundException("This deck does not contain any cards");
            }

            // Retrieve the cards by their IDs
            List<Card> cards = await _cardContext.Cards
                .Where(c => cardIDs.Contains(c.CardID) && c.DeletedAt == null)
                .ToListAsync();

            return cards;
        }
    }
}
