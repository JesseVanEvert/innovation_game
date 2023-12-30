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
    public class CardServiceTests
    {
        private EnvironmentVariables env = new EnvironmentVariables();
        private Mock<ICardRepository> _cardRepoMock;
        private CardService _cardServiceMock;
        private List<Model.Entity.Card> _cardListMock;

        private Guid mockCardId1 = Guid.NewGuid();
        private Guid mockCardId2 = Guid.NewGuid();
        private Guid mockDeckId = Guid.NewGuid();


        [SetUp]
        public void Setup()
        {
            // Set mock environment variables
            env.SetMockVariables();


            _cardRepoMock = new Mock<ICardRepository>();
            _cardServiceMock = new CardService(_cardRepoMock.Object);
            _cardListMock = new List<Model.Entity.Card>();

            Model.Entity.Card mockCard1 = new Model.Entity.Card()
            {
                CardID = mockCardId1,
                FrontSideText = "Mock Card 1",
                BackSideText = "Enge Steff",
                ImageUrl = null,
                DeletedAt = null
            };
            Model.Entity.Card mockCard2 = new Model.Entity.Card()
            {
                CardID = mockCardId2,
                FrontSideText = "Mock Card 2",
                BackSideText = "Grappige Steff",
                ImageUrl = null,
                DeletedAt = null
            };

            _cardListMock.Add(mockCard1);
            _cardListMock.Add(mockCard2);     
        }

        [Test]
        public async Task Calling_GetCard_Returns_Correct_Card_As_Card()
        {
            // Arrange
            _cardRepoMock.Setup(m => m.GetCardFromDeckAsync(mockCardId2)).ReturnsAsync(_cardListMock[1]);

            // Act
            Model.Entity.Card result = await _cardServiceMock.GetCardFromDeckAsync(mockCardId2);

            // Assert
            Assert.AreEqual(result.BackSideText, _cardListMock[1].BackSideText);
            Assert.That(result, Is.InstanceOf(typeof(Model.Entity.Card)));
            _cardRepoMock.Verify(m => m.GetCardFromDeckAsync(mockCardId2), Times.Once);
        }

        [Test]
        public async Task Calling_CreateCard_Adds_Card_To_Deck_And_Returns_CardId()
        {
            
            Guid newMockCardId = Guid.NewGuid();

            CardBaseWithDeckIDDTO mockDTO = new CardBaseWithDeckIDDTO()
            {
                DeckID = mockDeckId,
                FrontSideText = "Melancholische Steff",
                BackSideText = "MockBacksideText",
            };

            // Arrange
            _cardRepoMock.Setup(m => m.CreateCardInDeckAsync(It.IsAny<Model.Entity.Card>(), It.IsAny<Guid>()))
                .Callback(new Action<Model.Entity.Card, Guid>((x, y) =>
                {
                    _cardListMock.Add(x);
                }));

            // Act
            Guid result = await _cardServiceMock.CreateCardInDeckAsync(mockDTO);

            // Assert                        
            // Cannot check CardId vs GUID result because it is generated in the logic and thus cannot be hardcoded
            Assert.AreEqual(result, _cardListMock[2].CardID); 
            Assert.That(result, Is.InstanceOf(typeof(Guid)));
            _cardRepoMock.Verify(m => m.CreateCardInDeckAsync(It.IsAny<Model.Entity.Card>(), It.IsAny<Guid>()), Times.Once);
        }


        [Test]
        public async Task Calling_UpdateCard_Updates_Correct_Card_Information()
        {
            // Set index to random number
            int index = 33;
            // Mock team to update existing Team
            string mockFrontText = "Updated mock";
            string mockBackText = "Updated Steff";
            CardBaseWithCardIDDTO mockDTO = new CardBaseWithCardIDDTO()
            {
                CardID = mockCardId2,
                FrontSideText = mockFrontText,
                BackSideText = mockBackText,            
            };

            _cardRepoMock.Setup(m => m.UpdateCardInDeckAsync(It.IsAny<CardBaseWithCardIDDTO>()))
                .Callback(new Action<Model.DTO.CardBaseWithCardIDDTO>( x =>
                {
                    for (int i = 0; i < _cardListMock.Count(); i++)
                    {
                        if (_cardListMock[i].CardID == mockDTO.CardID)
                        {
                            Model.Entity.Card updatedCard = new Model.Entity.Card()
                            {
                                CardID = x.CardID,
                                FrontSideText = x.FrontSideText,
                                BackSideText = x.BackSideText,
                                DeletedAt = null,
                            };
                            _cardListMock[i] = updatedCard;
                            index = i;
                        }
                    }
                }));

            await _cardServiceMock.UpdateCardInDeckAsync(mockDTO);

            Assert.AreEqual(mockDTO.FrontSideText, _cardListMock[index].FrontSideText);
            _cardRepoMock.Verify(c => c.UpdateCardInDeckAsync(It.IsAny<CardBaseWithCardIDDTO>()), Times.Once);
        }

        [Test]
        public async Task Calling_DeleteCard_Sets_DeletedAt_Property_To_Current_DateTime()
        {
            int index = 33;
            CardWithCardIDAndDeckIDDTO mockDTO = new CardWithCardIDAndDeckIDDTO()
            {
                CardID = mockCardId1,
                DeckID = Guid.NewGuid(),
            };
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
            await _cardServiceMock.DeleteCardInDeckAsync(mockDTO);

            Assert.AreEqual(_cardListMock[index].DeletedAt, DateTime.Now.Date);
            _cardRepoMock.Verify(c => c.DeleteCardInDeckAsync(It.IsAny<CardWithCardIDAndDeckIDDTO>()), Times.Once);
        }


        [TearDown]
        public void TestCleanUp()
        {
            _cardRepoMock = null;
            _cardServiceMock = null;
            _cardListMock = null;
        }


    }
}

