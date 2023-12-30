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

using UserTeamOrg.BLL.Interfaces;
using UserTeamOrg.Model.DTO;
using UserTeamOrg.DAL;
using UserTeamOrg.DAL.IRepository;


namespace UserTeamOrg.Testing.Repository
{
    public class UserRepositoryTests
    {
        private Mock<IUserRepository> _userRepoMock;
        private IUserRepository _userRepo;
        private List<Model.Entity.User> _userListMock;

        private int index;
        private Guid _userIdMock1 = Guid.NewGuid();
        private Guid _userIdMock2 = Guid.NewGuid();
        private Guid _personIdMock1 = Guid.NewGuid();
        private Guid _personIdMock2 = Guid.NewGuid();
        private Guid _newUserIdMock = Guid.NewGuid();
        private Guid _newPersonIdMock = Guid.NewGuid();
        private Guid _teamIdMock = Guid.NewGuid();

        [SetUp]
        public void Setup()
        {
            _userRepoMock = new Mock<IUserRepository>();


            Model.Entity.User mockUser1 = new Model.Entity.User()
            {
                UserId = _userIdMock1,
                PersonId = _personIdMock1,
                Person = new Model.Entity.Person()
                {
                    PersonId = _personIdMock1,
                    Email = "mockuser1@email.com",
                    FirstName = "mock1",
                    LastName = "user1",
                    Password = "Secret",
                    DeletedAt = null
                },
                DeletedAt = null
            };
            Model.Entity.User mockUser2 = new Model.Entity.User()
            {
                UserId = _userIdMock2,
                PersonId = _personIdMock2,
                Person = new Model.Entity.Person()
                {
                    PersonId = _personIdMock2,
                    Email = "mockuser2@email.com",
                    FirstName = "mock2",
                    LastName = "user2",
                    Password = "Secret",
                    DeletedAt = null
                },
                DeletedAt = null
            };
            _userListMock = new List<Model.Entity.User>();
            _userListMock.Add(mockUser1);
            _userListMock.Add(mockUser2);

            // GetUser
            _userRepoMock.Setup(m => m.GetUser(It.Is<Guid>(i => i == _userIdMock1)))
                .Returns<Guid>(r => new Model.Entity.User
                {
                    UserId = r,
                    PersonId = _personIdMock1,
                    Person = new Model.Entity.Person()
                    {
                        PersonId = _personIdMock1,
                        Email = "mockuser1@email.com",
                        FirstName = "mock1",
                        LastName = "user1",
                        Password = "Secret",
                        DeletedAt = null
                    },
                    DeletedAt = null
                });

            // RegisterUser
            _userRepoMock.Setup(m => m.RegisterUser(It.IsAny<UserBaseWithTeamIDDTO>(), It.IsAny<Guid>()))
                .ReturnsAsync("newMockUser@email.com")
                .Callback(new Action<UserBaseWithTeamIDDTO, Guid>((x,y) =>
                {
                    Model.Entity.User newUser = new Model.Entity.User()
                    {
                        UserId = y,
                        PersonId = _newPersonIdMock,
                        Person = new Model.Entity.Person()
                        {
                            PersonId = _newPersonIdMock,
                            Email = x.Email,
                            FirstName = x.FirstName,
                            LastName = x.LastName,
                            Password = x.Password,
                            DeletedAt = null,
                        },
                        DeletedAt = null,
                    };
                    _userListMock.Add(newUser);
                }));

            // UpdateUser
            _userRepoMock.Setup(m => m.UpdateUser(It.IsAny<UserDTO>()))
                .Callback(new Action<UserDTO>(x =>
                {
                    for (int i = 0; i < _userListMock.Count(); i++)
                    {
                        if (_userListMock[i].UserId == x.UserId)
                        {
                            Model.Entity.User newUser = new Model.Entity.User()
                            {
                                UserId = _userListMock[i].UserId,
                                PersonId = _userListMock[i].PersonId,
                                Person = new Model.Entity.Person()
                                {
                                    Email = x.Email,
                                    FirstName = x.FirstName,
                                    LastName = x.LastName,
                                    Password = x.Password,
                                    ImageUrl = x.ImageUrl,
                                }
                            };
                            _userListMock[i] = newUser;
                            index = i;
                        }
                    }
                }));

            // DeleteUser
            _userRepoMock.Setup(m => m.DeleteUser(It.Is<Guid>(i => i == _userIdMock1)))
                .Callback(new Action<Guid>(x =>
                {
                    for (int i = 0; i < _userListMock.Count(); i++)
                    {
                        if (_userListMock[i].UserId == x)
                        {
                            _userListMock[i].DeletedAt = DateTime.Now.Date;
                            index = i;
                        }
                    }
                }));

            _userRepo = _userRepoMock.Object;
        }

        [Test]
        public void Calling_GetUser_Returns_Correct_User_As_User()
        {
            // Arrange

            // Act
            Model.Entity.User? result = _userRepo.GetUser(_userIdMock1);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(result.UserId, _userListMock[0].UserId);
            Assert.That(result, Is.InstanceOf(typeof(Model.Entity.User)));
        }

        [Test]
        public async Task Calling_RegisterUser_Adds_User_And_Returns_Email()
        {
            // Arrange
            UserBaseWithTeamIDDTO registerUserDTO = new UserBaseWithTeamIDDTO()
            {
                TeamId = _teamIdMock,
                Email = "newMockUser@email.com",
                FirstName = "new mock",
                LastName = "Fresh of the Boat",
                Password = "NewSecret",
            };

            // Act
            string result = await _userRepo.RegisterUser(registerUserDTO, _newUserIdMock);

            // Assert            
            Assert.IsNotNull(result);
            Assert.AreEqual(result, _userListMock[2].Person.Email);
            Assert.AreEqual(_newUserIdMock, _userListMock[2].UserId);
            Assert.That(result, Is.InstanceOf(typeof(string)));
            _userRepoMock.Verify(c => c.RegisterUser(It.IsAny<UserBaseWithTeamIDDTO>(), It.IsAny<Guid>()), Times.Once);
        }

        [Test]
        public async Task Calling_UpdateUser_Changes_User_information_R()
        {
            // Arrange            
            UserDTO updateUserDTO = new UserDTO()
            {
                UserId = _userIdMock2,
                DeletedAt = null,
                FirstName = "updated mock user",
                LastName = "updated mock",
                Email = "updatedemail@mockymockmoq.com",
                ImageUrl = "funnypics.gov/fbi-funniest",
                Password = "NewSECRET"
            };

            // Act
            await _userRepo.UpdateUser(updateUserDTO);

            // Assert                        
            Assert.AreEqual(updateUserDTO.FirstName, _userListMock[index].Person.FirstName);

        }

        [Test]
        public async Task Calling_DeleteUser_Changes_DeletedAt_Property_To_Current_DateTime()
        {
            // Arrange

            // Act
            await _userRepo.DeleteUser(_userIdMock1);

            // Assert
            Assert.AreEqual(_userListMock[index].DeletedAt, DateTime.Now.Date);
        }


        [TearDown]
        public void TestCleanUp()
        {
            _userRepo = null;
            _userRepoMock = null;
            _userListMock = null;
            index = 33;
        }
    }
}

