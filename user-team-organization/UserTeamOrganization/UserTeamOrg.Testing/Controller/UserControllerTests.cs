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
using User.API.Controllers;


namespace UserTeamOrg.Testing.Controller

{
    public class UserControllerTests
    {
        private Mock<IUserService> _userServiceMock;
        private Mock<ILogger<UserController>> mockLogger;
        private Mock<ITokenService> mockTokenService;
        private List<Model.DTO.UserDTO> _userDTOListMock;
        private List<Model.DTO.UserBaseWithTeamIDDTO> _userTeamIdListMock;
        private Dictionary<string, string> mockAuthResponse;

        // Generate Guids here so we can check them later
        private Guid _mockTeamId = Guid.NewGuid();
        private Guid _mockUserId1 = Guid.NewGuid();
        private Guid _mockPersonId1 = Guid.NewGuid();
        private Guid _mockUserId2 = Guid.NewGuid();
        private Guid _mockPersonId2 = Guid.NewGuid();
        private Guid newMockUserId = Guid.NewGuid();
        private Guid newMockPersonId = Guid.NewGuid();
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
            // Create mock tokenService and mock authResponse
            mockAuthResponse = new Dictionary<string, string>();
            mockTokenService = new Mock<ITokenService>();

            _userServiceMock = new Mock<IUserService>();
            mockLogger = new Mock<ILogger<UserController>>();

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

            // Mock the response of the tokenservice
            mockTokenService.Setup(fu => fu.GetRequestAuth(It.IsAny<HttpRequest>()))
                .ReturnsAsync(mockAuthResponse);

            _userServiceMock.Setup(r => r.GetUser(It.Is<Guid>(i => i == _mockUserId1))).Returns<Guid>(
                r => _userDTOListMock[0]);

            _userServiceMock.Setup(r => r.RegisterUser(It.IsAny<Model.DTO.UserBaseWithTeamIDDTO>()))
                .ReturnsAsync("latestMockUser@mockmail.com")
                .Callback(new Action<Model.DTO.UserBaseWithTeamIDDTO>(x =>
                    {
                        Model.DTO.UserDTO newUser = new Model.DTO.UserDTO()
                        {
                            UserId = newMockUserId,
                            PersonId = newMockPersonId,
                            Email = x.Email,
                            FirstName = x.FirstName,
                            LastName = x.LastName,
                            Password = x.Password,
                            ImageUrl = null,
                            DeletedAt = null,
                        };
                        _userDTOListMock.Add(newUser);
                    }));

            _userServiceMock.Setup(r => r.UpdateUser(It.IsAny<Model.DTO.UserDTO>()))
                .Callback(new Action<Model.DTO.UserDTO>(x =>
                {
                    for (int i = 0; i < _userDTOListMock.Count; i++)
                    {
                        if (_userDTOListMock[i].UserId == x.UserId)
                        {
                            _userDTOListMock[i] = x;
                            index = i;
                        }
                    }
                }));

            _userServiceMock.Setup(r => r.DeleteUser(It.Is<Guid>(i => i == _mockUserId1)))
                .Callback(new Action<Guid>(x =>
                {
                    for (int i = 0; i < _userDTOListMock.Count; i++)
                    {
                        if (_userDTOListMock[i].UserId == x)
                        {
                            _userDTOListMock[i].DeletedAt = DateTime.Now.Date;
                            index = i;
                        }
                    }
                }));

        }

        /*[Test]
        public async Task Calling_GetUser_Returns_OK_With_User_As_UserDTO()
        {
            // Arrange
            UserController mockUserController = new UserController(mockLogger.Object, _userServiceMock.Object, mockTokenService.Object);
            var qc = new QueryCollection(new Dictionary<string, StringValues> { { "userId", new StringValues(_mockUserId1.ToString()) } });
            _mockRequest = new Mock<HttpRequest>();           
            _mockRequest.Setup(x => x.Query).Returns(() => qc);  

            // Act
            var result = await mockUserController.GetUser(_mockRequest.Object);
            var OkResult = result as OkObjectResult;
            UserDTO resultUser = (UserDTO)OkResult.Value;
            // Assert
            Assert.IsNotNull(result);
            Assert.IsNotNull(OkResult);
            Assert.AreEqual(resultUser.UserId, _userDTOListMock[0].UserId);
            Assert.AreEqual(200, OkResult.StatusCode);

        }*/

        /*[Test]
        public async Task Calling_RegisterUser_Return_CREATED_With_Email_Of_Registered_User()
        {
            UserController mockUserController = new UserController(mockLogger.Object, _userServiceMock.Object, mockTokenService.Object);
            Guid mockId = Guid.NewGuid();
            Model.DTO.UserBaseWithTeamIDDTO newMockUserWithTeamIdDTO = new Model.DTO.UserBaseWithTeamIDDTO()
            {
                TeamId = mockId,
                Email = "latestMockUser@mockmail.com",
                FirstName = "Jordy",
                LastName = "Gekkehuis"
            };
            _mockRequest = CreateMockRequest(newMockUserWithTeamIdDTO);

            // Act
            var result = await mockUserController.RegisterUser(_mockRequest.Object);
            var CREATEDResult = result as CreatedResult;

            // Assert      
            Assert.IsNotNull(result);
            Assert.IsNotNull(CREATEDResult);
            Assert.AreEqual(CREATEDResult.Value, _userDTOListMock[2].Email);
            Assert.AreEqual(201, CREATEDResult.StatusCode);
            _userServiceMock.Verify(c => c.RegisterUser(It.IsAny<UserBaseWithTeamIDDTO>()), Times.Once);
            _memoryStream.Dispose();

        }*/

        /*[Test]
        public async Task Calling_UpdateUser_Returns_OK_And_Updates_User()
        {
            UserController mockUserController = new UserController(mockLogger.Object, _userServiceMock.Object, mockTokenService.Object);
            Model.DTO.UserDTO updateUser = new Model.DTO.UserDTO()
            {
                UserId = _mockUserId2,
                Email = "updatedUser@email.com",
                FirstName = "Updated",
                LastName = "Jorde",
                Password = "UpdatedSecret",
                DeletedAt = null
            };

            _mockRequest = CreateMockRequest(updateUser);

            var result = await mockUserController.UpdateUser(_mockRequest.Object);
            var OkResult = result as OkObjectResult;


            Assert.IsNotNull(result);
            Assert.IsNotNull(OkResult);
            Assert.AreEqual(updateUser.Email, _userDTOListMock[1].Email);
            Assert.AreEqual(200, OkResult.StatusCode);
            _userServiceMock.Verify(c => c.UpdateUser(It.IsAny<UserDTO>()), Times.Once);
        }*/

        [Test]
        public async Task Calling_DeleteUser_Returns_OK_And_Sets_DeletedAt_Property_To_DateTimeNow()
        {
            UserController mockUserController = new UserController(mockLogger.Object, _userServiceMock.Object, mockTokenService.Object);
            var qc = new QueryCollection(new Dictionary<string, StringValues> { { "userId", new StringValues(_mockUserId1.ToString()) } });
            _mockRequest = new Mock<HttpRequest>();
            _mockRequest.Setup(x => x.Query).Returns(() => qc);

            var result = await mockUserController.DeleteUser(_mockRequest.Object);
            var OkResult = result as OkObjectResult;


            _userServiceMock.Verify(c => c.DeleteUser(_mockUserId1), Times.Once);
            Assert.IsNotNull(result);
            Assert.IsNotNull(OkResult);
            Assert.AreEqual(200, OkResult.StatusCode);
        }

        [TearDown]
        public void CleanUp()
        {
            _userServiceMock = null;
            mockLogger = null;
            _mockRequest = null;
            _userDTOListMock = null;
            _userTeamIdListMock = null;
            index = 33;

        }

    }
}

