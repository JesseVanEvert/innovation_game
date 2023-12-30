
﻿using Game.DAL.Interfaces;
using Game.Exceptions.Exceptions;
using Game.Models.DTO;
using Game.Models.Entity;
using Microsoft.EntityFrameworkCore;

namespace Game.DAL.Repository
{
    public class DeckRepository : IDeckRepository
    {

        private readonly GameContext _gameContext;
        public DeckRepository(GameContext cardContext)
        {
            _gameContext = cardContext;
        }

        public async Task CreateDeckAsync(Deck newDeck, TeamDeck teamDeck)
        {
            //adding the new deck to the Deck table 
            await _gameContext.Decks.AddAsync(newDeck);
            
            //Adding the teamDeck entity to the corresponding table
            await _gameContext.TeamDecks.AddAsync(teamDeck);
            //Saving the changes to the database
            await _gameContext.SaveChangesAsync();
        }

        public async Task<List<Card>> GetAllCardsFromDeckAsync(Guid deckID)
        {
            List<CardDeck> cardDecks = await _gameContext.CardDecks.Where(c => c.DeckID == deckID).ToListAsync();
            List<Card> cards = new(); 

            foreach (var cardDeck in cardDecks)
            {
                Card card = await _gameContext.Cards.Where(c => c.CardID == cardDeck.CardID).FirstAsync();
                cards.Add(card);
            }

            return cards;
        }        

        public async Task DeleteDeckFromTeamAsync(DeckWithDeckIDAndTeamIDDTO deck)
        {
            //finding the to be deleted deck in the database
            Deck deckToDelete = await _gameContext.Decks
                //if DeletedAt value is not null, it means that this deck was deleted.
                .Where(d => d.DeckID == deck.DeckID && d.DeletedAt == null)
                .FirstOrDefaultAsync() ?? throw new EntityNotFoundException("Could not find a deck with this ID");
            //updating the DeletedAt property to the current time to keep track of when the record was deleted, since we never want to delete a record for data analysis
            deckToDelete.DeletedAt = DateTime.Now;

            //saving the changes.
            await _gameContext.SaveChangesAsync();

            //finding the corresponding teamDeck entry of the deck that was deleted
            TeamDeck teamDeckToDelete = await _gameContext.TeamDecks
                .Where(t => t.DeckID == deck.DeckID && t.TeamID == deck.TeamID && t.DeletedAt == null)
                .FirstOrDefaultAsync() ?? throw new EntityNotFoundException("This deck was not part of this team");
            //updating the DeletedAt property to the current time to keep track of when the record was deleted, since we never want to delete a record for data analysis
            teamDeckToDelete.DeletedAt = DateTime.Now;

            //saving the changes to the database
            await _gameContext.SaveChangesAsync();
        }

        public async Task UpdateDeckAsync(DeckBaseWithIDDTO deck)
        {
            //finding the deck that should be updated.
            Deck deckToUpdate = await _gameContext.Decks
                .Where(d => d.DeckID == deck.DeckID && d.DeletedAt == null)
                .FirstOrDefaultAsync() ?? throw new EntityNotFoundException("Could not find a Deck with this ID");
            //setting the values of the updated deck to the to be updated deck
            _gameContext.Entry(deckToUpdate).CurrentValues.SetValues(deck);

            //saving the changes to the database
            await _gameContext.SaveChangesAsync();
        }
    }
}
