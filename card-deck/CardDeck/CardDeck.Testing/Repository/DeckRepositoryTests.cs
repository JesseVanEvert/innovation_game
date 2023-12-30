using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Specialized;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.Extensions.Primitives;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;

using CardDeck.BLL.Interfaces;
using CardDeck.Model.DTO;
using CardDeck.Model.Entity;
using CardDeck.DAL.RepositoryInterface;

namespace CardDeck.Testing.Repository
{
    public class DeckRepositoryTests
    {
        private Mock<IDeckRepository> _deckRepoMock;
        private IDeckRepository _deckRepo;
        private List<Model.Entity.Deck> _deckListMock;

        // Generate Guids here so we can check them later
        private Guid _newDeckId = Guid.NewGuid();
        private Guid _mockDeckId1 = Guid.NewGuid();
        private Guid _mockDeckId2 = Guid.NewGuid();
        private Guid _teamIdMock = Guid.NewGuid();
        private int index;

        [SetUp]
        public void Setup()
        {
            _deckRepoMock = new Mock<IDeckRepository>();

            _deckListMock = new List<Model.Entity.Deck>();
            Model.Entity.Deck deck1 = new Model.Entity.Deck()
            {
                DeckID = _mockDeckId1,
                Name = "mock deck 1",
                DeletedAt = null
            };
            Model.Entity.Deck deck2 = new Model.Entity.Deck()
            {
                DeckID = _mockDeckId1,
                Name = "mock deck 2",
                DeletedAt = null
            };
            _deckListMock.Add(deck1);
            _deckListMock.Add(deck2);

            // GetDeck
            _deckRepoMock.Setup(m => m.GetDeckAsync(_mockDeckId1, _teamIdMock))
                .ReturnsAsync(_deckListMock[0]);

            // GetAllDecks
            _deckRepoMock.Setup(m => m.GetAllDecksAsync(_teamIdMock))
                .ReturnsAsync(_deckListMock);

            // CreateDeck
            _deckRepoMock.Setup(m => m.CreateDeckAsync(It.IsAny<Model.Entity.Deck>(), It.IsAny<Guid>()))
                .Callback(new Action<Model.Entity.Deck, Guid>((x, y) =>
                {
                    _deckListMock.Add(x);
                }));

            // UpdateDeck
            _deckRepoMock.Setup(m => m.UpdateDeckAsync(It.IsAny<Model.DTO.DeckBaseWithIDDTO>()))
                .Callback(new Action<Model.DTO.DeckBaseWithIDDTO>(x =>
                {
                    for (int i = 0; i < _deckListMock.Count(); i++)
                    {
                        if (_deckListMock[i].DeckID == x.DeckID)
                        {
                            _deckListMock[i].Name = x.Name;
                            index = i;
                        }
                    }
                }));

            // DeleteDeck
            _deckRepoMock.Setup(m => m.DeleteDeckFromTeamAsync(It.IsAny<DeckWithDeckIDAndTeamIDDTO>()))
                .Callback(new Action<DeckWithDeckIDAndTeamIDDTO>(x =>
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

            _deckRepo = _deckRepoMock.Object;
        }

        [Test]
        public async Task Calling_GetDeck_Returns_Correct_Deck_As_Deck()
        {
            // Arrange

            // Act
            Model.Entity.Deck? result = await _deckRepo.GetDeckAsync(_mockDeckId1, _teamIdMock);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(result.DeckID, _deckListMock[0].DeckID);
            Assert.That(result, Is.InstanceOf(typeof(Model.Entity.Deck)));
            _deckRepoMock.Verify(c => c.GetDeckAsync(_mockDeckId1, _teamIdMock), Times.Once);
        }


        [Test]
        public async Task Calling_GetAll_Decks_Returns_List_Of_Decks()
        {
            // Arrange

            // Act
            List<Model.Entity.Deck> result = await _deckRepo.GetAllDecksAsync(_teamIdMock);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(result, _deckListMock);
            Assert.That(result, Is.InstanceOf(typeof(List<Model.Entity.Deck>)));
            _deckRepoMock.Verify(c => c.GetAllDecksAsync(_teamIdMock), Times.Once);
        }

        [Test]
        public async Task Calling_CreateDeck_Adds_Deck()
        {
            // Arrange
            Model.Entity.Deck newDeck = new Model.Entity.Deck()
            {
                DeckID = _newDeckId,
                Name = "new mock deck",
                DeletedAt = null
            };


            // Act
            await _deckRepo.CreateDeckAsync(newDeck, _teamIdMock);

            // Assert            
            Assert.AreEqual(_newDeckId, _deckListMock[2].DeckID);
            _deckRepoMock.Verify(c => c.CreateDeckAsync(It.IsAny<Model.Entity.Deck>(), It.IsAny<Guid>()), Times.Once);
        }

        [Test]
        public async Task Calling_UpdateDeck_Changes_Deck_information_R()
        {
            // Arrange            
            DeckBaseWithIDDTO updateDeck = new DeckBaseWithIDDTO()
            {
                Name = "Updated mock deck",
                DeckID = _mockDeckId1,
                teamID = _teamIdMock,
            };

            // Act
            await _deckRepo.UpdateDeckAsync(updateDeck);

            // Assert                        
            Assert.AreEqual(updateDeck.Name, _deckListMock[index].Name);
            _deckRepoMock.Verify(c => c.UpdateDeckAsync(It.IsAny<DeckBaseWithIDDTO>()), Times.Once);
        }

        [Test]
        public async Task Calling_DeleteDeck_Changes_DeletedAt_Property_To_Current_DateTime()
        {
            // Arrange
            DeckWithDeckIDAndTeamIDDTO deleteDeck = new DeckWithDeckIDAndTeamIDDTO()
            {
                DeckID = _mockDeckId1,
                TeamID = _teamIdMock,
            };

            // Act
            await _deckRepo.DeleteDeckFromTeamAsync(deleteDeck);

            // Assert
            Assert.AreEqual(_deckListMock[index].DeletedAt, DateTime.Now.Date);
            _deckRepoMock.Verify(c => c.DeleteDeckFromTeamAsync(It.IsAny<DeckWithDeckIDAndTeamIDDTO>()), Times.Once);
        }


        [TearDown]
        public void TestCleanUp()
        {
            _deckRepo = null;
            _deckRepoMock = null;
            _deckListMock = null;
            index = 33;
        }
    }
}

