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
    public class CardRepositoryTests
    {
        private Mock<ICardRepository> _cardRepoMock;
        private ICardRepository _cardRepo;
        private List<Model.Entity.Card> _cardListMock;

        // Generate Guids here so we can check them later
        private Guid _newCardId = Guid.NewGuid();
        private Guid _mockCardId1 = Guid.NewGuid();
        private Guid _mockCardId2 = Guid.NewGuid();

        private Guid _mockDeckId1 = Guid.NewGuid();
        private Guid _mockDeckId2 = Guid.NewGuid();
        private Guid _teamIdMock = Guid.NewGuid();

        private int index;

        [SetUp]
        public void Setup()
        {
            _cardRepoMock = new Mock<ICardRepository>();

            _cardListMock = new List<Model.Entity.Card>();
            Model.Entity.Card mockCard1 = new Model.Entity.Card()
            {
                CardID = _mockCardId1,
                FrontSideText = "Mock Card 1",
                BackSideText = "Enge Steff",
                ImageUrl = null,
                DeletedAt = null
            };
            Model.Entity.Card mockCard2 = new Model.Entity.Card()
            {
                CardID = _mockCardId2,
                FrontSideText = "Mock Card 2",
                BackSideText = "Grappige Steff",
                ImageUrl = null,
                DeletedAt = null
            };
            _cardListMock.Add(mockCard1);
            _cardListMock.Add(mockCard2);

            // GetCard
            _cardRepoMock.Setup(m => m.GetCardFromDeckAsync(It.Is<Guid>(i => i == _mockCardId1)))
                .ReturnsAsync(_cardListMock[0]);

            // CreateCard
            _cardRepoMock.Setup(m => m.CreateCardInDeckAsync(It.IsAny<Model.Entity.Card>(), It.IsAny<Guid>()))
                .Callback(new Action<Model.Entity.Card, Guid>((x, y) =>
                {
                    _cardListMock.Add(x);                    
                }));

            // UpdateCard
            _cardRepoMock.Setup(m => m.UpdateCardInDeckAsync(It.IsAny<Model.DTO.CardBaseWithCardIDDTO>()))
                .Callback(new Action<Model.DTO.CardBaseWithCardIDDTO>(x =>
                {
                    for (int i = 0; i < _cardListMock.Count(); i++)
                    {
                        if (_cardListMock[i].CardID == x.CardID)
                        {
                            _cardListMock[i].FrontSideText = x.FrontSideText;
                            _cardListMock[i].BackSideText = x.BackSideText;
                            index = i;
                        }
                    }
                }));

            // DeleteCard
            _cardRepoMock.Setup(m => m.DeleteCardInDeckAsync(It.IsAny<CardWithCardIDAndDeckIDDTO>()))
                .Callback(new Action<CardWithCardIDAndDeckIDDTO>(x =>
                {
                    for (int i = 0; i < _cardListMock.Count(); i++)
                    {
                        if (_cardListMock[i].CardID == x.CardID)
                        {
                            _cardListMock[i].DeletedAt = DateTime.Now.Date;
                            index = i;
                        }
                    }
                }));

            _cardRepo = _cardRepoMock.Object;
        }

        [Test]
        public async Task Calling_GetCard_Returns_Correct_Card_As_Card()
        {
            // Arrange

            // Act
            Model.Entity.Card? result = await _cardRepo.GetCardFromDeckAsync(_mockCardId1);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(result.CardID, _cardListMock[0].CardID);
            Assert.That(result, Is.InstanceOf(typeof(Model.Entity.Card)));
            _cardRepoMock.Verify(c => c.GetCardFromDeckAsync(_mockCardId1), Times.Once);
        }

        [Test]
        public async Task Calling_CreateCard_Adds_Card()
        {
            // Arrange
            Model.Entity.Card newCard = new Model.Entity.Card()
            {
                CardID = _newCardId,
                FrontSideText = "new Mock Card",
                BackSideText = "new Grappige Steff",
                ImageUrl = null,
                DeletedAt = null
            };

            // Act
            await _cardRepo.CreateCardInDeckAsync(newCard, _mockDeckId1);

            // Assert            
            Assert.AreEqual(_newCardId, _cardListMock[2].CardID);
            _cardRepoMock.Verify(c => c.CreateCardInDeckAsync(It.IsAny<Model.Entity.Card>(), It.IsAny<Guid>()), Times.Once);
        }

        [Test]
        public async Task Calling_UpdateCard_Changes_Card_information_R()
        {
            // Arrange            
            CardBaseWithCardIDDTO updateCard = new CardBaseWithCardIDDTO()
            {
                CardID = _mockCardId2,
                FrontSideText = "updated Mock Card 2",
                BackSideText = "updated Steff",

            };

            // Act
            await _cardRepo.UpdateCardInDeckAsync(updateCard);

            // Assert                        
            Assert.AreEqual(updateCard.FrontSideText, _cardListMock[index].FrontSideText);
            _cardRepoMock.Verify(c => c.UpdateCardInDeckAsync(It.IsAny<CardBaseWithCardIDDTO>()), Times.Once);
        }

        [Test]
        public async Task Calling_DeleteCard_Changes_DeletedAt_Property_To_Current_DateTime()
        {
            // Arrange
            CardWithCardIDAndDeckIDDTO deleteCard = new CardWithCardIDAndDeckIDDTO()
            {
                DeckID = _mockDeckId1,
                CardID = _mockCardId1,
            };

            // Act
            await _cardRepo.DeleteCardInDeckAsync(deleteCard);

            // Assert
            Assert.AreEqual(_cardListMock[index].DeletedAt, DateTime.Now.Date);
            _cardRepoMock.Verify(c => c.DeleteCardInDeckAsync(It.IsAny<CardWithCardIDAndDeckIDDTO>()), Times.Once);
        }


        [TearDown]
        public void TestCleanUp()
        {
            _cardRepo = null;
            _cardRepoMock = null;
            _cardListMock = null;
            index = 33;
        }

    }
}

