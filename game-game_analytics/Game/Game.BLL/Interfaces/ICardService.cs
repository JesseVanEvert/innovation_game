using Game.Models.DTO;

namespace Game.BLL.Interfaces
{
    public interface ICardService
    {
        Task CreateCardInDeckAsync(CardBaseWithCardIDAndDeckIDDTO cardBase);
        Task UpdateCardInDeckAsync(CardBaseWithCardIDDTO cardBase);
        Task DeleteCardInDeckAsync(CardWithCardIDAndDeckIDDTO cardBase);
    }
}
