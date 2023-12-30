using Game.BLL;
using Game.BLL.Interfaces;
using Game.DAL.Interfaces;
using Game.DAL.Repository;
using Game.Models.DTO;
using Game.Models.Entity;
using Microsoft.Extensions.Caching.Distributed;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game.Tests.Service
{
    public class GameServiceTests
    {
        private GameService _gameService;
        private Mock<IDistributedCache> _mockCache;
        private Mock<IDeckRepository> _mockDeckRepo;
        private Mock<IUserRepository> _userRepository;
        private Mock<IGameRepository> _gameRepository;
        private Mock<UserRepository> _mockUserRepo;

        private List<CreateGameBodyDTO> _existingGamesMock;
        private List<GameUserBodyDTO> _playerListMock;
        private List<Guid> _readyPlayersMock;
        private List<Card> _cards;
        private Models.Entity.User _jan;
        private List<Models.Entity.User> _userList;

        private Guid _userIdMock1 = Guid.NewGuid();
        private Guid _gameIdMock1 = Guid.NewGuid();
        private Guid _userIdMock2 = Guid.NewGuid();
        private Guid _gameIdMock2 = Guid.NewGuid();
        private Guid _deckIdMock = Guid.NewGuid();

        private Guid newGameId = Guid.NewGuid();
        private Guid _newPlayerUserId = Guid.NewGuid();
        private Guid _newPlayerGameId = Guid.NewGuid();


        [SetUp]
        public void Setup()
        {
            _mockDeckRepo = new Mock<IDeckRepository>();
            _mockCache = new Mock<IDistributedCache>();
            _userRepository = new Mock<IUserRepository>();
            _gameRepository = new Mock<IGameRepository>();
            _gameService = new(_mockCache.Object, _mockDeckRepo.Object, _userRepository.Object, _gameRepository.Object);

            _playerListMock = new List<GameUserBodyDTO>();
            GameUserBodyDTO player1 = new GameUserBodyDTO()
            {
                UserID = _userIdMock1,
                GameID = _gameIdMock1,
            };
            GameUserBodyDTO player2 = new GameUserBodyDTO()
            {
                UserID = _userIdMock2,
                GameID = _gameIdMock2,
            };
            _playerListMock.Add(player1);
            _playerListMock.Add(player2);

            _readyPlayersMock = new List<Guid>();
            _readyPlayersMock.Add(_userIdMock1);

            _jan = new Models.Entity.User() { UserID = Guid.NewGuid(), Email = "jan@hotmail.com", Firstname = "Jan", Lastname = "Vries" };
            _userList = new() { _jan };
            _cards = new()
            {
                new Card() {CardID = Guid.NewGuid(), FrontSideText= "tekst", BackSideText="tekst"},
                new Card() {CardID = Guid.NewGuid(), FrontSideText= "tekst", BackSideText="tekst"}
            };

            _existingGamesMock = new List<CreateGameBodyDTO>();
        }
  
        [Test]
        public async Task Calling_CreateGame_Returns_GameId()
        {
            // Arrange
            _mockDeckRepo.Setup(m => m.GetAllCardsFromDeckAsync(It.IsAny<Guid>()))
                .ReturnsAsync(_cards);

            CreateGameBodyDTO dto = new()
            {
                DeckID = Guid.NewGuid(),
                Name = "Mock Game"
            };

            // Act
            Guid result = await _gameService.CreateGame(dto);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(result.GetType(), typeof(Guid));
        }
        /*
  [Test]
  public async Task Calling_JoinGame_Returns_Message()
  {
      _userRepository.Setup(u => u.GetUserAsync(It.IsAny<Guid>()))
          .ReturnsAsync(_jan);



      GameUserBodyDTO dto = new()
      {
          GameID = Guid.NewGuid(),
          UserID = Guid.NewGuid(),
      };


      // Act
      string result = await _gameService.JoinGame(dto);

      // Assert
      Assert.IsNotNull(result);
      Assert.That(result, Is.InstanceOf(typeof(string)));
  }

  /*
  [Test]
  public async Task Calling_PlayerIsReady_Returns_Message()
  {
      // arrange
      GameUserBodyDTO dto = new()
      {
          GameID = _gameIdMock1,
          UserID = _userIdMock1,
      };

      // act
      string result = await _gameService.PlayerIsReady(dto);

      // assert
      Assert.IsNotNull(result);
      Assert.That(result, Is.InstanceOf(typeof(string)));
  }

  [Test]
  public async Task Calling_LeaveGame_Returns_Message()
  {
      // Arrange
      Guid newPlayer = Guid.NewGuid();
      GameUserBodyDTO dto = new GameUserBodyDTO()
      {
          GameID = _gameIdMock1,
          UserID = newPlayer,
      };

      // Act
      string result = await _gameService.LeaveGame(dto);

      // Assert
      Assert.IsNotNull(result);
      Assert.That(result, Is.InstanceOf(typeof(string)));
  }

  [TearDown]
  public void TestCleanUp()
  {
      _gameService = null;
      _mockCache = null;
      _mockDeckRepo = null;
      _mockUserRepo = null;
      _existingGamesMock = null;
      _playerListMock = null;
      _readyPlayersMock = null;
  }
  */
    }
}
