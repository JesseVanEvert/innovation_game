using CardDeck.Model.DTO;
using CardDeck.Model.Entity;

namespace CardDeck.BLL.Interfaces
{
    public interface IDeckService
    {
        Task<Guid> CreateDeckAsync(DeckBaseDTO deck);
        Task UpdateDeckAsync(DeckBaseWithIDDTO deck);
        Task DeleteDeckFromTeamAsync(DeckWithDeckIDAndTeamIDDTO deck);
        Task<Deck> GetDeckAsync(Guid deckID, Guid teamID);
        Task<List<Deck>> GetAllDecksAsync(Guid teamID);
    }
}
