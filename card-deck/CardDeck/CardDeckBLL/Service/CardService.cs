using CardDeck.BLL.Interfaces;
using CardDeck.DAL.RepositoryInterface;
using CardDeck.Model.DTO;
using CardDeck.Model.Entity;
using Azure.Storage.Queues;
using Newtonsoft.Json;
using CardDeck.BLL.Service;

namespace CardDeck.BLL
{
    public class CardService : ICardService
    {
        private readonly ICardRepository _cardRepository;
        private readonly QueueClient queue;

        public CardService(ICardRepository cardRepository)
        {
            _cardRepository = cardRepository;
            queue = QueueService.GetQueueClient();
        }
        public async Task<Guid> CreateCardInDeckAsync(CardBaseWithDeckIDDTO cardBase)
        {
            //creating the card entity with the given values
            Card card = new()
            {
                CardID = Guid.NewGuid(),
                FrontSideText = cardBase.FrontSideText,
                BackSideText = cardBase.BackSideText,
                DeletedAt = null
            };

            //Imageurl is not required in the call, so can be null
            if (cardBase.ImageUrl != null)
            {
                card.ImageUrl = cardBase.ImageUrl;
            }

            await _cardRepository.CreateCardInDeckAsync(card, cardBase.DeckID);

            /*
                if there were no exceptions when executing the CreateCardInDeck function, a message with the object is send via the queue
                to be put in the replicationdatabase in the Game repository
                To preserve the CardID we have to send a slightly different object WITH the newly generated cardID in the message.
            */
            CardBaseWithCardIDAndDeckIDDTO newCardbase = new()
            {
                FrontSideText = cardBase.FrontSideText,
                BackSideText = cardBase.BackSideText,
                DeckID = cardBase.DeckID,
                CardID = card.CardID
            };
            //Imageurl is not required in the call, so can be null
            if (cardBase.ImageUrl != null)
            {
                newCardbase.ImageUrl = cardBase.ImageUrl;
            }
            KeyValuePair<string, string> queueMessage = new("CreateCard", JsonConvert.SerializeObject(newCardbase));
            await QueueService.AddMessageAsJsonAsync(queueMessage, queue);

            return card.CardID;
        }

        public async Task DeleteCardInDeckAsync(CardWithCardIDAndDeckIDDTO cardBase)
        {
            await _cardRepository.DeleteCardInDeckAsync(cardBase);
            /*
                if there were no exceptions when executing the DeleteCardInDeck function, a message with the object is send via the queue
                to be put in the replicationdatabase in the Game repository
            */
            KeyValuePair<string, string> queueMessage = new("DeleteCard", JsonConvert.SerializeObject(cardBase));
            await QueueService.AddMessageAsJsonAsync(queueMessage, queue);
        }

        public async Task<Card> GetCardFromDeckAsync(Guid cardID)
        {
            return await _cardRepository.GetCardFromDeckAsync(cardID);
        }

        public async Task UpdateCardInDeckAsync(CardBaseWithCardIDDTO cardBase)
        {
            await _cardRepository.UpdateCardInDeckAsync(cardBase);
            /*
                if there were no exceptions when executing the UpdateCardInDeck function, a message with the object is send via the queue
                to be put in the replicationdatabase in the Game repository
            */
            KeyValuePair<string, string> queueMessage = new("UpdateCard", JsonConvert.SerializeObject(cardBase));
            await QueueService.AddMessageAsJsonAsync(queueMessage, queue);
        }

        //Get all cards from a deck
        public async Task<List<Card>> GetAllCardsFromDeckAsync(Guid deckID)
        {
            return await _cardRepository.GetAllCardsFromDeckAsync(deckID);
        }
    }
}
