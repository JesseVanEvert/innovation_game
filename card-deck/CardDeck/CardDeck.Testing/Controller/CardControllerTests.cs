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
using Card.API.Controller;
using CardDeck.Model.Entity;
using Deck.API.Controller;

namespace CardDeck.Testing.Controller
{
    public class CardControllerTests
    {
        private Mock<ICardService> _cardServiceMock;
        private Mock<ILogger<CardControllers>> mockLogger;
        private Mock<ITokenService> mockTokenService;
        private List<Model.Entity.Card> _cardListMock;
        private Dictionary<string, string> mockAuthResponse;

        // Generate Guids here so we can check them later
        private Guid _newCardId = Guid.NewGuid();
        private Guid _mockCardId1 = Guid.NewGuid();
        private Guid _mockCardId2 = Guid.NewGuid();

        private Guid _mockDeckId1 = Guid.NewGuid();
        private Guid _mockDeckId2 = Guid.NewGuid();        
        private Guid _teamIdMock = Guid.NewGuid();

        private int index;

        private MemoryStream _memoryStream;
        private Mock<HttpRequest> _mockRequest;


        private Mock<HttpRequest> CreateMockRequest(object body)
        {
            var json = JsonConvert.SerializeObject(body);
            var bytesArray = Encoding.ASCII.GetBytes(json);

            _memoryStream = new MemoryStream(bytesArray);
            _memoryStream.Flush();
            _memoryStream.Position = 0;

            var mockRequest = new Mock<HttpRequest>();
            mockRequest.Setup(x => x.Body).Returns(_memoryStream);

            return mockRequest;
        }


        [SetUp]
        public void Setup()
        {
            mockTokenService = new Mock<ITokenService>();
            mockAuthResponse = new Dictionary<string, string>();

            _cardServiceMock = new Mock<ICardService>();
            mockLogger = new Mock<ILogger<CardControllers>>();

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


            mockTokenService.Setup(fu => fu.GetRequestAuth(It.IsAny<HttpRequest>()))
                .ReturnsAsync(mockAuthResponse);

            // GetCard
            _cardServiceMock.Setup(r => r.GetCardFromDeckAsync(It.IsAny<Guid>()))
                .ReturnsAsync(_cardListMock[0]);

            // CreateCard
            _cardServiceMock.Setup(r => r.CreateCardInDeckAsync(It.IsAny<CardBaseWithDeckIDDTO>()))
                .ReturnsAsync(It.IsAny<Guid>())
                .Callback(new Action<CardBaseWithDeckIDDTO>(x =>
                {
                    Model.Entity.Card newCard = new Model.Entity.Card()
                    {
                        CardID = _newCardId,
                        FrontSideText = "new Mock Card 2",
                        BackSideText = "Nieuwe Steff",
                        DeletedAt = null,
                        ImageUrl = "image",
                    };
                    _cardListMock.Add(newCard);
                }));

            // UpdateCard
            _cardServiceMock.Setup(r => r.UpdateCardInDeckAsync(It.IsAny<CardBaseWithCardIDDTO>()))
                .Callback(new Action<CardBaseWithCardIDDTO>(x =>
                {
                    for (int i = 0; i < _cardListMock.Count; i++)
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
            _cardServiceMock.Setup(r => r.DeleteCardInDeckAsync(It.IsAny<CardWithCardIDAndDeckIDDTO>()))
                .Callback(new Action<CardWithCardIDAndDeckIDDTO>(x =>
                {
                    for (int i = 0; i < _cardListMock.Count; i++)
                    {
                        if (_cardListMock[i].CardID == x.CardID)
                        {
                            _cardListMock[i].DeletedAt = DateTime.Now.Date;
                            index = i;
                        }
                    }
                }));
        }

        [Test]
        public async Task Calling_GetCard_Returns_OK_With_Card_As_Card()
        {
            // Arrange
            CardControllers mockCardController = new CardControllers(mockLogger.Object, _cardServiceMock.Object, mockTokenService.Object);
            var qc = new QueryCollection(new Dictionary<string, StringValues> {
                { "CardID", new StringValues(_mockCardId1.ToString()) }});

            _mockRequest = new Mock<HttpRequest>();
            _mockRequest.Setup(x => x.Query).Returns(() => qc);

            // Act
            var result = await mockCardController.GetCardFromDeck(_mockRequest.Object);
            var OkResult = result as OkObjectResult;
            Model.Entity.Card resultCard = (Model.Entity.Card)OkResult.Value;
            // Assert
            Assert.IsNotNull(result);
            Assert.IsNotNull(OkResult);
            Assert.AreEqual(resultCard.CardID, _cardListMock[0].CardID);
            Assert.AreEqual(200, OkResult.StatusCode);
        }

        [Test]
        public async Task Calling_CreateCard_Return_CREATED_With_CardId()
        {
            // Arrange
            CardControllers mockCardController = new CardControllers(mockLogger.Object, _cardServiceMock.Object, mockTokenService.Object);
            Guid mockId = Guid.NewGuid();
            CardBaseWithDeckIDDTO dto = new CardBaseWithDeckIDDTO()
            {
                DeckID = _mockDeckId1,
                FrontSideText = "new Mock Card 2",
                BackSideText = "Nieuwe Steff",
            };
            _mockRequest = CreateMockRequest(dto);

            // Act
            var result = await mockCardController.CreateCard(_mockRequest.Object);
            var OKResult = result as OkObjectResult;
            Guid resultValue = (Guid)OKResult.Value;
            // Assert
            _cardServiceMock.Verify(c => c.CreateCardInDeckAsync(It.IsAny<CardBaseWithDeckIDDTO>()), Times.Once);
            Assert.IsNotNull(result);
            Assert.IsNotNull(OKResult);
            Assert.AreEqual(OKResult.Value, resultValue);
            Assert.AreEqual(200, OKResult.StatusCode);

            _memoryStream.Dispose();
        }

        [Test]
        public async Task Calling_UpdateCard_Returns_OK_And_Updates_Card()
        {
            // Arrange
            CardControllers mockCardController = new CardControllers(mockLogger.Object, _cardServiceMock.Object, mockTokenService.Object);
            CardBaseWithCardIDDTO dto = new CardBaseWithCardIDDTO()
            {
                CardID = _mockCardId2,
                FrontSideText = "updated Mock Card 2",
                BackSideText = "Nieuwe Steff",

            };

            _mockRequest = CreateMockRequest(dto);

            // Act
            var result = await mockCardController.UpdateCard(_mockRequest.Object);
            var OkResult = result as OkObjectResult;

            // Assert
            _cardServiceMock.Verify(c => c.UpdateCardInDeckAsync(It.IsAny<CardBaseWithCardIDDTO>()), Times.Once);
            Assert.IsNotNull(result);
            Assert.IsNotNull(OkResult);
            Assert.AreEqual(200, OkResult.StatusCode);
        }

        [Test]
        public async Task Calling_DeleteCard_Returns_OK_And_Sets_DeletedAt_Property_To_DateTimeNow()
        {
            // Arrange
            CardControllers mockCardController = new CardControllers(mockLogger.Object, _cardServiceMock.Object, mockTokenService.Object);
            CardWithCardIDAndDeckIDDTO dto = new CardWithCardIDAndDeckIDDTO()
            {
                DeckID = _mockDeckId1,
                CardID = _mockCardId1,                
            };

            _mockRequest = CreateMockRequest(dto);

            // Act
            var result = await mockCardController.DeleteCardFromDeck(_mockRequest.Object);
            var OkResult = result as OkObjectResult;

            // Assert            
            Assert.IsNotNull(result);
            Assert.IsNotNull(OkResult);
            Assert.AreEqual(200, OkResult.StatusCode);
            _cardServiceMock.Verify(c => c.DeleteCardInDeckAsync(It.IsAny<CardWithCardIDAndDeckIDDTO>()), Times.Once);
        }

        [TearDown]
        public void CleanUp()
        {
            _cardServiceMock = null;
            mockLogger = null;
            _mockRequest = null;
            _cardListMock = null;
            index = 33;

        }
    }
}

