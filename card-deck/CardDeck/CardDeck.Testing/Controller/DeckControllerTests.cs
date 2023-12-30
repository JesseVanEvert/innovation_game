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
using Deck.API.Controller;
using CardDeck.Model.Entity;

namespace CardDeck.Testing.Controller
{
    public class DeckControllerTests
    {
        private Mock<IDeckService> _deckServiceMock;
        private Mock<ILogger<DeckControllers>> mockLogger;
        private Mock<ITokenService> mockTokenService;
        private List<Model.Entity.Deck> _deckListMock;
        private Dictionary<string, string> mockAuthResponse;

        // Generate Guids here so we can check them later
        private Guid _mockDeckId1 = Guid.NewGuid();
        private Guid _mockDeckId2 = Guid.NewGuid();
        private Guid _newDeckId = Guid.NewGuid();
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

            _deckServiceMock = new Mock<IDeckService>();
            mockLogger = new Mock<ILogger<DeckControllers>>();

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

            mockTokenService.Setup(fu => fu.GetRequestAuth(It.IsAny<HttpRequest>()))
                .ReturnsAsync(mockAuthResponse);

            mockTokenService.Setup(fu => fu.CheckTeamRole(It.IsAny<Dictionary<string, string>>(), It.IsAny<Role>(), It.IsAny<Guid>()))
                .Returns(true);

            // GetDeck
            _deckServiceMock.Setup(r => r.GetDeckAsync(_mockDeckId1, _teamIdMock))
                .ReturnsAsync(_deckListMock[0]);

            // GetAllDecks
            _deckServiceMock.Setup(r => r.GetAllDecksAsync(It.IsAny<Guid>()))
                .ReturnsAsync(_deckListMock);

            // CreateDeck
            _deckServiceMock.Setup(r => r.CreateDeckAsync(It.IsAny<DeckBaseDTO>()))
                .ReturnsAsync(It.IsAny<Guid>())
                .Callback(new Action<DeckBaseDTO>(x =>
                {
                    Model.Entity.Deck newDeck = new Model.Entity.Deck()
                    {
                        DeckID = _newDeckId,
                        Name = "latest mock deck",
                        DeletedAt = null,
                    };
                    _deckListMock.Add(newDeck);
                }));

            // UpdateDeck
            _deckServiceMock.Setup(r => r.UpdateDeckAsync(It.IsAny<DeckBaseWithIDDTO>()))
                .Callback(new Action<DeckBaseWithIDDTO>(x =>
                {
                    for (int i = 0; i < _deckListMock.Count; i++)
                    {
                        if (_deckListMock[i].DeckID == x.DeckID)
                        {
                            _deckListMock[i].Name = x.Name;
                            index = i;
                        }
                    }
                }));

            // DeleteDeck
            _deckServiceMock.Setup(r => r.DeleteDeckFromTeamAsync(It.IsAny<DeckWithDeckIDAndTeamIDDTO>()))
                .Callback(new Action<DeckWithDeckIDAndTeamIDDTO>(x =>
                {
                    for (int i = 0; i < _deckListMock.Count; i++)
                    {
                        if (_deckListMock[i].DeckID == x.DeckID)
                        {
                            _deckListMock[i].DeletedAt = DateTime.Now.Date;
                            index = i;
                        }
                    }
                }));
        }

        [Test]
        public async Task Calling_GetDeck_Returns_OK_With_Deck_As_Deck()
        {
            // Arrange
            DeckControllers mockDeckController = new DeckControllers(mockLogger.Object, _deckServiceMock.Object, mockTokenService.Object);
            var qc = new QueryCollection(new Dictionary<string, StringValues> {
                { "DeckID", new StringValues(_mockDeckId1.ToString()) },
                { "TeamID", new StringValues(_teamIdMock.ToString())  } });

            _mockRequest = new Mock<HttpRequest>();
            _mockRequest.Setup(x => x.Query).Returns(() => qc);

            // Act
            var result = await mockDeckController.GetDeck(_mockRequest.Object);
            var OkResult = result as OkObjectResult;
            Model.Entity.Deck resultDeck = (Model.Entity.Deck)OkResult.Value;
            // Assert
            Assert.IsNotNull(result);
            Assert.IsNotNull(OkResult);
            Assert.AreEqual(resultDeck.DeckID, _deckListMock[0].DeckID);
            Assert.AreEqual(200, OkResult.StatusCode);
        }

        [Test]
        public async Task Calling_GetAllDecks_Returns_OK_With_All_Decks()
        {
            // Arrange
            DeckControllers mockDeckController = new DeckControllers(mockLogger.Object, _deckServiceMock.Object, mockTokenService.Object);
            var qc = new QueryCollection(new Dictionary<string, StringValues>
            {
                {"TeamID", new StringValues(_teamIdMock.ToString()) }
            });

            _mockRequest = new Mock<HttpRequest>();
            _mockRequest.Setup(x => x.Query).Returns(() => qc);

            // Act
            var result = await mockDeckController.GetAllDecks(_mockRequest.Object);
            var OkResult = result as OkObjectResult;
            List<Model.Entity.Deck> resultList = (List<Model.Entity.Deck>)OkResult.Value;

            // Assert
            Assert.IsNotNull(result);
            Assert.IsNotNull(OkResult);
            Assert.AreEqual(resultList, _deckListMock);
            Assert.AreEqual(200, OkResult.StatusCode);
        }

        [Test]
        public async Task Calling_CreateDeck_Return_CREATED_With_DeckId()
        {
            // Arrange
            DeckControllers mockDeckController = new DeckControllers(mockLogger.Object, _deckServiceMock.Object, mockTokenService.Object);
            Guid mockId = Guid.NewGuid();
            Model.DTO.DeckBaseDTO dto = new Model.DTO.DeckBaseDTO()
            {                
                Name = "latest mock team",
                teamID = _teamIdMock,
            };
            _mockRequest = CreateMockRequest(dto);

            // Act
            var result = await mockDeckController.CreateDeck(_mockRequest.Object);
            var OKResult = result as OkObjectResult;
            Guid resultValue = (Guid)OKResult.Value;
            // Assert
            _deckServiceMock.Verify(c => c.CreateDeckAsync(It.IsAny<DeckBaseDTO>()), Times.Once);
            Assert.IsNotNull(result);
            Assert.IsNotNull(OKResult);
            Assert.AreEqual(OKResult.Value, resultValue);
            Assert.AreEqual(200, OKResult.StatusCode);

            _memoryStream.Dispose();
        }

        [Test]
        public async Task Calling_UpdateDeck_Returns_OK_And_Updates_Deck()
        {
            // Arrange
            DeckControllers mockDeckController = new DeckControllers(mockLogger.Object, _deckServiceMock.Object, mockTokenService.Object);
            DeckBaseWithIDDTO dto = new DeckBaseWithIDDTO()
            {
                Name = "Updated mock deck",
                DeckID = _mockDeckId1,
                teamID = _teamIdMock,
            };

            _mockRequest = CreateMockRequest(dto);

            // Act
            var result = await mockDeckController.UpdateCard(_mockRequest.Object);
            var OkResult = result as OkObjectResult;            

            // Assert
            _deckServiceMock.Verify(c => c.UpdateDeckAsync(It.IsAny<DeckBaseWithIDDTO>()), Times.Once);
            Assert.IsNotNull(result);
            Assert.IsNotNull(OkResult);            
            Assert.AreEqual(200, OkResult.StatusCode);
        }

        [Test]
        public async Task Calling_DeleteDeck_Returns_OK_And_Sets_DeletedAt_Property_To_DateTimeNow()
        {
            // Arrange
            DeckControllers mockDeckController = new DeckControllers(mockLogger.Object, _deckServiceMock.Object, mockTokenService.Object);
            DeckWithDeckIDAndTeamIDDTO dto = new DeckWithDeckIDAndTeamIDDTO()
            {
                DeckID = _mockDeckId1,
                TeamID = _teamIdMock,
            };

            _mockRequest = CreateMockRequest(dto);            

            // Act
            var result = await mockDeckController.DeleteDeck(_mockRequest.Object);
            var OkResult = result as OkObjectResult;

            // Assert
            _deckServiceMock.Verify(c => c.DeleteDeckFromTeamAsync(It.IsAny<DeckWithDeckIDAndTeamIDDTO>()), Times.Once);
            Assert.IsNotNull(result);
            Assert.IsNotNull(OkResult);
            Assert.AreEqual(200, OkResult.StatusCode);
        }

        [TearDown]
        public void CleanUp()
        {
            _deckServiceMock = null;
            mockLogger = null;
            _mockRequest = null;
            _deckListMock = null;
            index = 33;

        }
    }
}

