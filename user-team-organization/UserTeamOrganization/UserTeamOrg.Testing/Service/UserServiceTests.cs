using Moq;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using UserTeamOrg.BLL;
using Microsoft.AspNetCore.Mvc;

using UserTeamOrg.DAL.IRepository;
using UserTeamOrg.DAL;
using UserTeamOrg.Model;
using UserTeamOrg.Exceptions;

namespace UserTeamOrg.Testing.Service
{    
    public class UserServiceTests
    {
        private EnvironmentVariables env = new EnvironmentVariables();        
        private Mock<IUserRepository> _userRepoMock;
        private UserService _userServiceMock;
        private List<UserTeamOrg.Model.DTO.UserDTO> _userDTOListMock;
        private List<UserTeamOrg.Model.Entity.User> _usersListMock;
        
        // Generate Guids here so we can check them later
        private Guid _mockTeamId = Guid.NewGuid();
        private Guid _mockUserId1 = Guid.NewGuid();
        private Guid _mockPersonId1 = Guid.NewGuid();
        private Guid _mockUserId2 = Guid.NewGuid();
        private Guid _mockPersonId2 = Guid.NewGuid();

        [SetUp]
        public void Setup()
        {
            // Set mock environment variables
            env.SetMockVariables();

            // Set up mock service
            _userRepoMock = new Mock<IUserRepository>();
            _userServiceMock = new UserService(_userRepoMock.Object);

            // Set up mock UserDTO list for GetUser
            _userDTOListMock = new List<Model.DTO.UserDTO>();
            Model.DTO.UserDTO user1 = new Model.DTO.UserDTO()
            {
                UserId = _mockUserId1,
                PersonId = _mockPersonId1,                
                Email = "mockuser1@email.com",
                FirstName = "mock1",
                LastName = "user1",
                Password = "Secret",
                DeletedAt = null
            };
            Model.DTO.UserDTO user2 = new Model.DTO.UserDTO()
            {
                UserId = _mockUserId2,
                PersonId = _mockPersonId2,
                Email = "mockuser2@email.com",
                FirstName = "mock2",
                LastName = "user2",
                Password = "Secret",
                DeletedAt = null
            };

            _userDTOListMock.Add(user1);
            _userDTOListMock.Add(user2);

            // Set up mock User that Repo is supposed to return
            _usersListMock = new List<Model.Entity.User>();
            Model.Entity.User user = new Model.Entity.User()
            {
                UserId = _mockUserId1,
                PersonId = _mockPersonId1,
                Person = new Model.Entity.Person()
                {
                    PersonId = _mockPersonId1,
                    Email = "mockuser1@email.com",
                    FirstName = "mock1",
                    LastName = "user1",
                    Password = "Secret",
                    DeletedAt = null
                },
                DeletedAt = null
            };

            _usersListMock.Add(user);

        }

        [Test]
        public void Calling_GetUser_Returns_Correct_User_As_UserDTO()
        {
            // Arrange
            _userRepoMock.Setup(m => m.GetUser(_mockUserId1)).Returns<Guid>(
                r => _usersListMock[0]);
                
            // Act
            Model.DTO.UserDTO result = _userServiceMock.GetUser(_mockUserId1);

            // Assert
            Assert.AreEqual(result.FirstName, _userDTOListMock[0].FirstName);
            Assert.That(result, Is.InstanceOf(typeof(Model.DTO.UserDTO)));

            _userRepoMock.Verify(c => c.GetUser(_mockUserId1), Times.Once);
        }

        /*[Test]
        public async Task Calling_RegisterUser_Adds_User_And_Returns_Email()
        {
            // new Mock DTO to register
            Model.DTO.UserBaseWithTeamIDDTO mockDTO = new Model.DTO.UserBaseWithTeamIDDTO()
            {
                TeamId = _mockTeamId,
                Email = "newestMockUser@email.com",
                FirstName = "new",
                LastName = "mock",
                Password = "SUPERSECRET"
            };
            // Arrange
            _userRepoMock.Setup(m => m.RegisterUser(It.IsAny<Model.DTO.UserBaseWithTeamIDDTO>(), It.IsAny<Guid>()))
                .ReturnsAsync(mockDTO.Email)
                .Callback(new Action<Model.DTO.UserBaseWithTeamIDDTO, Guid>((x,y) =>
            {
                Model.Entity.User newUser = new Model.Entity.User()
                {
                    UserId = Guid.NewGuid(),
                    PersonId = Guid.NewGuid(),
                    Person = new Model.Entity.Person()
                    {
                        FirstName = x.FirstName,
                        LastName = x.LastName,
                        Email = x.Email,
                        Password = x.Password,
                        ImageUrl = null,
                        DeletedAt = null,
                    },
                    DeletedAt = null,
                };
                _usersListMock.Add(newUser);
            }));

            // Act
            string result = await _userServiceMock.RegisterUser(mockDTO);

            // Assert
            Assert.AreEqual(result, _usersListMock[1].Person.Email);
            Assert.That(result, Is.InstanceOf(typeof(string)));
            _userRepoMock.Verify(c => c.RegisterUser(It.IsAny<Model.DTO.UserBaseWithTeamIDDTO>(), It.IsAny<Guid>()), Times.Once);
        }*/

        /*[Test]
        public async Task Calling_UpdateUser_Changes_User_information()
        {
            // Set index to random number
            int index = 5;
            // Mock DTO to update mock user
            Model.DTO.UserDTO mockDTO = new Model.DTO.UserDTO()
            {
                UserId = _mockUserId1,
                Email = "updatedmock@MoqMockMok.com",
                FirstName = "updatedMock1",
                LastName = "MockyMockMoq",
                ImageUrl = null,
                DeletedAt = null,
            };
            // Arrange
            _userRepoMock.Setup(m => m.UpdateUser(mockDTO)).Callback(new Action<Model.DTO.UserDTO>(x =>
            {                
                for (int i = 0; i < _userDTOListMock.Count(); i++)
                {
                    // Find the mock userDTO in the list to update
                    if (_userDTOListMock[i].UserId == x.UserId)
                    {
                        _userDTOListMock[i] = x;
                        // check if index will be properly updated
                        index = i;
                    }
                }
            }));
            // Act
            await _userServiceMock.UpdateUser(mockDTO);

            // Assert
            Assert.AreEqual(mockDTO.LastName, _userDTOListMock[index].LastName);            
            _userRepoMock.Verify(c => c.UpdateUser(mockDTO), Times.Once);

        }*/

        [Test]
        public async Task Calling_DeleteUser_Changes_DeletedAt_Property_To_Current_DateTime()
        {
            int index = 5;
            _userRepoMock.Setup(m => m.DeleteUser(_mockUserId1)).Callback(new Action<Guid>(x =>
            {
                for (int i = 0; i < _userDTOListMock.Count(); i++)
                {
                    if (_userDTOListMock[i].UserId == x)
                    {
                        _userDTOListMock[i].DeletedAt = DateTime.Now.Date;
                        
                        index = i;
                    }
                }
            }));

            await _userServiceMock.DeleteUser(_mockUserId1);

            Assert.AreEqual(_userDTOListMock[index].DeletedAt, DateTime.Now.Date);            
            _userRepoMock.Verify(c => c.DeleteUser(_mockUserId1), Times.Once);

        }

        [TearDown]
        public void TestCleanUp()
        {
            _userRepoMock = null;
            _userServiceMock = null;
            _userDTOListMock = null;
            _usersListMock = null;            
        }

    }
}

