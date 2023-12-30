using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;

using CardDeck.BLL;
using CardDeck.DAL.RepositoryInterface;
using CardDeck.DAL;
using CardDeck.Exceptions;
using CardDeck.Model;
using CardDeck.Model.DTO;
using CardDeck.Model.Entity;


namespace CardDeck.Testing.Service
{
    public class DeckServiceTests
    {
        private EnvironmentVariables env = new EnvironmentVariables();
        private Mock<IDeckRepository> _deckRepoMock;
        private DeckService _deckServiceMock;
        private List<Model.Entity.Deck> _deckListMock;
        private List<Guid> _teamIdListMock;

        private Guid mockDeckId1 = Guid.NewGuid();
        private Guid mockDeckId2 = Guid.NewGuid();

        private Guid mockTeamId1 = Guid.NewGuid();
        private Guid mockTeamId2 = Guid.NewGuid();


        [SetUp]
        public void Setup()
        {
            // Set mock environment variables
            env.SetMockVariables();

            _deckRepoMock = new Mock<IDeckRepository>();
            _deckServiceMock = new DeckService(_deckRepoMock.Object);
            _deckListMock = new List<Model.Entity.Deck>();

            Model.Entity.Deck mockDeck1 = new Model.Entity.Deck()
            {
                DeckID = mockDeckId1,
                Name = "Mock Deck 1",
                DeletedAt = null
            };
            Model.Entity.Deck mockDeck2 = new Model.Entity.Deck()
            {
                DeckID = mockDeckId2,
                Name = "Mock Deck 2",                
                DeletedAt = null
            };

            _deckListMock.Add(mockDeck1);
            _deckListMock.Add(mockDeck2);

            _teamIdListMock = new List<Guid>();
            _teamIdListMock.Add(mockTeamId1);
            _teamIdListMock.Add(mockTeamId2);

        }

        [Test]
        public async Task Calling_GetDeck_Returns_Correct_Deck_As_Deck()
        {
            // Arrange
            _deckRepoMock.Setup(m => m.GetDeckAsync(mockDeckId1, mockTeamId1)).ReturnsAsync(_deckListMock[0]);

            // Act
            Model.Entity.Deck result = await _deckServiceMock.GetDeckAsync(mockDeckId1, mockTeamId1);

            // Assert
            Assert.AreEqual(result.Name, _deckListMock[0].Name);
            Assert.That(result, Is.InstanceOf(typeof(Model.Entity.Deck)));
            _deckRepoMock.Verify(m => m.GetDeckAsync(mockDeckId1, mockTeamId1), Times.Once);
        }

        [Test]
        public async Task Calling_GetAllDecks_Returns_All_Decks()
        {
            // Arrange
            _deckRepoMock.Setup(m => m.GetAllDecksAsync(mockTeamId1)).ReturnsAsync(_deckListMock);

            // Act
            List<Model.Entity.Deck> results = await _deckServiceMock.GetAllDecksAsync(mockTeamId1);

            // Assert
            Assert.AreEqual(results, _deckListMock);
            Assert.That(results, Is.InstanceOf(typeof(List<Model.Entity.Deck>)));
            _deckRepoMock.Verify(m => m.GetAllDecksAsync(mockTeamId1), Times.Once);
        }

        [Test]
        public async Task Calling_CreateDeck_Creates_A_New_Deck_And_Returns_DeckId()
        {
            Guid newMockDeckId = Guid.NewGuid();
           
            DeckBaseDTO mockBase = new DeckBaseDTO()
            {
                Name = "New Mock Deck",
                teamID = mockTeamId1,
                
            };
            // Arrange
            _deckRepoMock.Setup(m => m.CreateDeckAsync(It.IsAny<Model.Entity.Deck>(), It.IsAny<Guid>()))
                .Callback(new Action<Model.Entity.Deck, Guid>((x, y) =>
                {
                    _deckListMock.Add(x);
                }));

            // Act
            await _deckServiceMock.CreateDeckAsync(mockBase);

            // Assert            
            Assert.AreEqual(mockBase.Name, _deckListMock[2].Name);            
            _deckRepoMock.Verify(m => m.CreateDeckAsync(It.IsAny<Model.Entity.Deck>(), It.IsAny<Guid>()), Times.Once);
        }


        [Test]
        public async Task Calling_UpdateDeck_Updates_Correct_Deck_Information()
        {
            // Set index to random number
            int index = 33;
            // Mock team to update existing Team
            DeckBaseWithIDDTO mockDTO = new DeckBaseWithIDDTO()
            {
                DeckID = mockDeckId1,
                teamID = mockTeamId1,
                Name = "Updated Mock Deck",
            };

            _deckRepoMock.Setup(m => m.UpdateDeckAsync(It.IsAny<DeckBaseWithIDDTO>()))
                .Callback(new Action<Model.DTO.DeckBaseWithIDDTO>(x =>
                {
                    for (int i = 0; i < _deckListMock.Count(); i++)
                    {
                        if (_deckListMock[i].DeckID == mockDTO.DeckID)
                        {
                            Model.Entity.Deck newDeck = new Model.Entity.Deck()
                            {
                                DeckID = x.DeckID,
                                Name = x.Name,
                                DeletedAt = null,
                            };
                            _deckListMock[i] = newDeck;
                            index = i;
                        }
                    }
                }));

            await _deckServiceMock.UpdateDeckAsync(mockDTO);

            Assert.AreEqual(mockDTO.Name, _deckListMock[index].Name);
            _deckRepoMock.Verify(c => c.UpdateDeckAsync(It.IsAny<DeckBaseWithIDDTO>()), Times.Once);
        }

        [Test]
        public async Task Calling_DeleteDeck_Sets_DeletedAt_Property_To_Current_DateTime()
        {
            int index = 33;
            DeckWithDeckIDAndTeamIDDTO mockDTO = new DeckWithDeckIDAndTeamIDDTO()
            {
                DeckID = mockDeckId1,
                TeamID = mockTeamId1,
                
            };
            _deckRepoMock.Setup(m => m.DeleteDeckFromTeamAsync(mockDTO)).Callback(new Action<DeckWithDeckIDAndTeamIDDTO>(x =>
            {
                for (int i = 0; i < _deckListMock.Count(); i++)
                {
                    if (_deckListMock[i].DeckID == x.DeckID)
                    {
                        _deckListMock[i].DeletedAt = DateTime.Now.Date;
                        index = i;
                    }
                }
            }));
            await _deckServiceMock.DeleteDeckFromTeamAsync(mockDTO);

            Assert.AreEqual(_deckListMock[index].DeletedAt, DateTime.Now.Date);
            _deckRepoMock.Verify(c => c.DeleteDeckFromTeamAsync(mockDTO), Times.Once);
        }


        [TearDown]
        public void TestCleanUp()
        {
            _deckRepoMock = null;
            _deckServiceMock = null;
            _deckListMock = null;
            _teamIdListMock = null;
        }

    }
}

