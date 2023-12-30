using Game.DAL.Interfaces;
using Game.Exceptions.Exceptions;
using Game.Models.DTO;
using Game.Models.Entity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game.DAL.Repository
{
    public class CardRepository : ICardRepository
    {

        private readonly GameContext _gameContext;
        public CardRepository(GameContext gameContext)
        {
            _gameContext = gameContext;
        }
        public async Task CreateCardInDeckAsync(Card card, CardDeck cardDeck)
        {
            //adding the card to the Card table
            await _gameContext.Cards.AddAsync(card);
            
            //adding the CardDeck entity to the CardDeck table
            await _gameContext.CardDecks.AddAsync(cardDeck);

            //saving the changes to the database
            await _gameContext.SaveChangesAsync();
        }

        public async Task DeleteCardInDeckAsync(CardWithCardIDAndDeckIDDTO cardBase)        
        {
            //finding the Card that has the given CardID
            Card? cardToDelete = await _gameContext.Cards
                .Where(c => c.CardID == cardBase.CardID && c.DeletedAt == null)
                .FirstOrDefaultAsync() ?? throw new EntityNotFoundException("Could not find a card with this ID");
            //updating the DeletedAt property to the current time to keep track of when the record was deleted, since we never want to delete a record for data analysis
            cardToDelete.DeletedAt = DateTime.Now;

            //saving the changes to the database
            await _gameContext.SaveChangesAsync();

            //finding the corresponding CardDeck entity which should be deleted
            CardDeck? cardDeckToDelete = await _gameContext.CardDecks
                .Where(c => c.CardID == cardBase.CardID && c.DeckID == cardBase.DeckID && c.DeletedAt == null)
                .FirstOrDefaultAsync() ?? throw new EntityNotFoundException("This Card does not exist in the given Deck");
            //updating the DeletedAt property to the current time to keep track of when the record was deleted, since we never want to delete a record for data analysis
            cardDeckToDelete.DeletedAt = DateTime.Now;

            //saving the changes to the database
            await _gameContext.SaveChangesAsync();
        }

        public async Task UpdateCardInDeckAsync(CardBaseWithCardIDDTO cardBase)
        {
            //finding the card that should be updated.
            Card cardToUpdate = await _gameContext.Cards
                .Where(c => c.CardID == cardBase.CardID && c.DeletedAt == null)
                .FirstOrDefaultAsync() ?? throw new EntityNotFoundException("Could not find a card with this ID");
            //updating the card that should be updated with the new values
            _gameContext.Entry(cardToUpdate).CurrentValues.SetValues(cardBase);

            //saving  the changes to the database
            await _gameContext.SaveChangesAsync();
        }
    }
}
