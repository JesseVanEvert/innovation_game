using Game.Models.DTO;
using Game.Models.Entity;

namespace Game.DAL.Interfaces
{
    public interface ICardRepository
    {
        Task CreateCardInDeckAsync(Card card, CardDeck cardDeck);
        Task UpdateCardInDeckAsync(CardBaseWithCardIDDTO cardBase);
        Task DeleteCardInDeckAsync(CardWithCardIDAndDeckIDDTO cardBase);
    }
}
