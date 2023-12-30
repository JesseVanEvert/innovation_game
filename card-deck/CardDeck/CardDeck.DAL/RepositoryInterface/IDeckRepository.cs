using CardDeck.Model.DTO;
using CardDeck.Model.Entity;

namespace CardDeck.DAL.RepositoryInterface
{
    public interface IDeckRepository
    {
        Task CreateDeckAsync(Deck newDeck, Guid teamID);
        Task UpdateDeckAsync(DeckBaseWithIDDTO deck);
        Task DeleteDeckFromTeamAsync(DeckWithDeckIDAndTeamIDDTO deck);
        Task<Deck> GetDeckAsync(Guid deckID, Guid teamID);
        Task<List<Deck>> GetAllDecksAsync(Guid teamID);
    }
}
