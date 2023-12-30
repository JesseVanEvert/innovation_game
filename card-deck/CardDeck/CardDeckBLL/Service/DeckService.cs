using Azure.Storage.Queues;
using CardDeck.BLL.Interfaces;
using CardDeck.BLL.Service;
using CardDeck.DAL.RepositoryInterface;
using CardDeck.Model.DTO;
using CardDeck.Model.Entity;
using Newtonsoft.Json;

namespace CardDeck.BLL
{
    public class DeckService : IDeckService
    {
        private readonly IDeckRepository _deckRepository;
        private readonly QueueClient queue;
        public DeckService(IDeckRepository deckRepository)
        {
            _deckRepository = deckRepository;
            queue = QueueService.GetQueueClient();
        }

        public async Task<Guid> CreateDeckAsync(DeckBaseDTO deckBase)
        {
            //creating new deckentity with given parameters
            Deck newDeck = new()
            {
                DeckID = Guid.NewGuid(),
                Name = deckBase.Name,
                DeletedAt = null
            };

            //calling the repository method to create the deck in the database with DBContext
            await _deckRepository.CreateDeckAsync(newDeck, deckBase.teamID);

            /*
                if there were no exceptions when executing the CreateDeckAsync function, a message with the object is send via the queue
                to be put in the replicationdatabase in the Game repository
                To preserve the DeckID we have to send a slightly different object WITH the newly generated deckid in the message.
            */
            DeckBaseWithIDDTO deckBaseWithID = new() {
                DeckID = newDeck.DeckID,
                Name = newDeck.Name,
                teamID = deckBase.teamID
            };

            KeyValuePair<string, string> queueMessage = new("CreateDeck", JsonConvert.SerializeObject(deckBaseWithID));
            await QueueService.AddMessageAsJsonAsync(queueMessage, queue);

            return newDeck.DeckID;
        }

        public async Task DeleteDeckFromTeamAsync(DeckWithDeckIDAndTeamIDDTO deck)
        {
            await _deckRepository.DeleteDeckFromTeamAsync(deck);

            /*
                if there were no exceptions when executing the DeleteDeckFromTeamAsync function, a message with the object is send via the queue
                to be put in the replicationdatabase in the Game repository
            */
            KeyValuePair<string, string> queueMessage = new("DeleteDeck", JsonConvert.SerializeObject(deck));
            await QueueService.AddMessageAsJsonAsync(queueMessage, queue);
        }

        public async Task<Deck> GetDeckAsync(Guid deckID, Guid teamID)
        {
            return await _deckRepository.GetDeckAsync(deckID, teamID);
        }

        public async Task<List<Deck>> GetAllDecksAsync(Guid teamID)
        {
            return await _deckRepository.GetAllDecksAsync(teamID);
        }

        public async Task UpdateDeckAsync(DeckBaseWithIDDTO deck)
        {
            await _deckRepository.UpdateDeckAsync(deck);

            /*
                if there were no exceptions when executing the UpdateDeckAsync function, a message with the object is send via the queue
                to be put in the replicationdatabase in the Game repository
            */
            KeyValuePair<string, string> queueMessage = new("UpdateDeck", JsonConvert.SerializeObject(deck));
            await QueueService.AddMessageAsJsonAsync(queueMessage, queue);
        }
    }
}
