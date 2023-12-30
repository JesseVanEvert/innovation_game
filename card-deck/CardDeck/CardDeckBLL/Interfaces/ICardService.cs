using CardDeck.Model.DTO;
using CardDeck.Model.Entity;

namespace CardDeck.BLL.Interfaces
{
    public interface ICardService
    {
        Task<Guid> CreateCardInDeckAsync(CardBaseWithDeckIDDTO cardBase);
        Task UpdateCardInDeckAsync(CardBaseWithCardIDDTO cardBase);
        Task DeleteCardInDeckAsync(CardWithCardIDAndDeckIDDTO cardBase);
        Task<Model.Entity.Card> GetCardFromDeckAsync(Guid cardID);
        Task<List<Card>> GetAllCardsFromDeckAsync(Guid deckID);
    }
}
