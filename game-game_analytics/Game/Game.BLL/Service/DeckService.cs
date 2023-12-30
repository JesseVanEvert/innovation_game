using Azure.Storage.Queues;
using Game.BLL.Interfaces;
using Game.BLL.Service;
using Game.DAL.Interfaces;
using Game.Models.DTO;
using Game.Models.Entity;
using Newtonsoft.Json;

namespace Game.BLL.Service
{
    public class DeckService : IDeckService
    {
        private readonly IDeckRepository _deckRepository;
        public DeckService(IDeckRepository deckRepository)
        {
            _deckRepository = deckRepository;
        }

        public async Task CreateDeckAsync(DeckBaseWithIDDTO deckBase)
        {
            //creating new deckentity with given parameters
            Deck newDeck = new()
            {
                DeckID = deckBase.DeckID,
                Name = deckBase.Name,
                DeletedAt = null
            };
            //Creating a new teamDeck entity to keep track of which team the new deck belongs to
            TeamDeck newTeamDeck = new()
            {
                DeckID = newDeck.DeckID,
                TeamID = deckBase.teamID
            };

            //calling the repository method to create the deck in the database with DBContext
            await _deckRepository.CreateDeckAsync(newDeck, newTeamDeck);
        }

        public async Task DeleteDeckFromTeamAsync(DeckWithDeckIDAndTeamIDDTO deck)
        {
            await _deckRepository.DeleteDeckFromTeamAsync(deck);
        }

        public async Task UpdateDeckAsync(DeckBaseWithIDDTO deck)
        {
            await _deckRepository.UpdateDeckAsync(deck);
        }
    }
}
