using CardDeck.Model.DTO;
using CardDeck.Model.Entity;

namespace CardDeck.DAL.RepositoryInterface
{
    public interface ICardRepository
    {
        Task CreateCardInDeckAsync(Card card, Guid deckID);
        Task UpdateCardInDeckAsync(CardBaseWithCardIDDTO cardBase);
        Task DeleteCardInDeckAsync(CardWithCardIDAndDeckIDDTO cardBase);
        Task<Card> GetCardFromDeckAsync(Guid cardID);
        Task<List<Card>> GetAllCardsFromDeckAsync(Guid deckID);
    }
}
