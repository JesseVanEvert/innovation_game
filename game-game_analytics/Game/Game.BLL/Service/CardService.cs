using Game.BLL.Interfaces;
using Game.DAL.Interfaces;
using Game.Models.DTO;
using Game.Models.Entity;
using Azure.Storage.Queues;
using Newtonsoft.Json;
using Game.BLL.Service;

namespace Game.BLL.Service
{
    public class CardService : ICardService
    {
        private readonly ICardRepository _cardRepository;

        public CardService(ICardRepository cardRepository)
        {
            _cardRepository = cardRepository;
        }
        public async Task CreateCardInDeckAsync(CardBaseWithCardIDAndDeckIDDTO cardBase)
        {
            //creating the card entity with the given values
            Card card = new()
            {
                CardID = cardBase.CardID,
                FrontSideText = cardBase.FrontSideText,
                BackSideText = cardBase.BackSideText,
                ImageUrl = cardBase.ImageUrl,
                DeletedAt = null
            };

            //creating the CardDeck entity to keep track of which deck the card belongs to
            CardDeck cardDeck = new()
            {
                CardID = card.CardID,
                DeckID = cardBase.DeckID,
                DeletedAt = null
            };

            await _cardRepository.CreateCardInDeckAsync(card, cardDeck);
        }

        public async Task DeleteCardInDeckAsync(CardWithCardIDAndDeckIDDTO cardBase)
        {
            await _cardRepository.DeleteCardInDeckAsync(cardBase);
        }

        public async Task UpdateCardInDeckAsync(CardBaseWithCardIDDTO cardBase)
        {
            await _cardRepository.UpdateCardInDeckAsync(cardBase);
        }
    }
}
