using Game.Models.DTO;
using Game.Models.Entity;

namespace Game.BLL.Interfaces
{
    public interface IDeckService
    {
        Task CreateDeckAsync(DeckBaseWithIDDTO deck);
        Task UpdateDeckAsync(DeckBaseWithIDDTO deck);
        Task DeleteDeckFromTeamAsync(DeckWithDeckIDAndTeamIDDTO deck);
    }
}
