using Game.Models.DTO;
using Game.Models.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game.DAL.Interfaces
{
    public interface IDeckRepository
    {
        Task CreateDeckAsync(Deck newDeck, TeamDeck teamDeck);
        Task UpdateDeckAsync(DeckBaseWithIDDTO deck);
        Task DeleteDeckFromTeamAsync(DeckWithDeckIDAndTeamIDDTO deck);
        Task<List<Card>> GetAllCardsFromDeckAsync(Guid deckID);
    }
}
