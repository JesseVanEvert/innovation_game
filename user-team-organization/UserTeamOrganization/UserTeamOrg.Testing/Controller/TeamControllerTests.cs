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
using Team.API.Controllers;

namespace UserTeamOrg.Testing.Controller

{
    public class TeamControllerTests
    {
        private Mock<IMailService> mockMailService;
        private Mock<ITokenService> mockTokenService;
        private Mock<ITeamService> _teamServiceMock;
        private Mock<ILogger<TeamController>> mockLogger;
        private List<Model.Entity.Team> _teamListMock;
        private List<TeamBaseWithIdDTO> _teamBaseListMock;
        private Dictionary<string, string> mockAuthResponse;


        // Generate Guids here so we can check them later
        private Guid _mockTeamId1 = Guid.NewGuid();
        private Guid _mockTeamId2 = Guid.NewGuid();
        private Guid _newlyRegisteredTeamId = Guid.NewGuid();
        private Guid _mockOrganizationId = Guid.NewGuid();
        private int index = 33;

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
            mockAuthResponse = new Dictionary<string, string>();
            mockMailService = new Mock<IMailService>();
            mockTokenService = new Mock<ITokenService>();
            _teamServiceMock = new Mock<ITeamService>();
            mockLogger = new Mock<ILogger<TeamController>>();

            _teamListMock = new List<Model.Entity.Team>();
            Model.Entity.Team team1 = new Model.Entity.Team()
            {
                TeamID = _mockTeamId1,
                Name = "mock team 1",
                DeletedAt = null
            };
            Model.Entity.Team team2 = new Model.Entity.Team()
            {
                TeamID = _mockTeamId2,
                Name = "mock team 2",
                DeletedAt = null
            };

            TeamBaseWithIdDTO updateTeam = new TeamBaseWithIdDTO()
            {
                Name = "Updated mock team",
                TeamId = _mockTeamId1,
                OrganizationId = _mockOrganizationId
            };

            TeamBaseDTO registerTeamDTO = new TeamBaseDTO()
            {
                Name = "latest mock team",
                OrganizationId = _mockOrganizationId
            };

            Model.Entity.Team newTeam = new Model.Entity.Team()
            {
                Name = "latest mock team",
                TeamID = _newlyRegisteredTeamId
            };

            _teamListMock.Add(team1);
            _teamListMock.Add(team2);

            mockTokenService.Setup(fu => fu.GetRequestAuth(It.IsAny<HttpRequest>()))
                .ReturnsAsync(mockAuthResponse);

            mockTokenService.Setup(fu => fu.CheckAccessLevel(It.IsAny<Dictionary<string, string>>(), It.IsAny<Model.Entity.AccessLevel>()))
                .Returns(true);

            mockMailService.Setup(fu => fu.SendEmail(It.IsAny<EmailDTO>()))
                .ReturnsAsync(true);


            // GetTeam
            _teamServiceMock.Setup(r => r.GetTeam(It.Is<Guid>(i => i == _mockTeamId1)))
                .ReturnsAsync(_teamListMock[0]);

            // CreateTeam
            _teamServiceMock.Setup(r => r.CreateTeam(It.IsAny<TeamBaseDTO>()))
                .ReturnsAsync(newTeam)
                .Callback(new Action<TeamBaseDTO>(x =>
                    {
                        newTeam.TeamID = newTeam.TeamID;
                        newTeam.Name = x.Name;
                        _teamListMock.Add(newTeam);
                    }));

            // UpdateTeam
            _teamServiceMock.Setup(r => r.UpdateTeam(It.IsAny<TeamBaseWithIdDTO>()))
                .Callback(new Action<TeamBaseWithIdDTO>(x =>
                {
                    for (int i = 0; i < _teamListMock.Count; i++)
                    {
                        if (_teamListMock[i].TeamID == x.TeamId)
                        {
                            _teamListMock[i].Name = x.Name;
                            index = i;
                        }
                    }
                }));

            // DeleteTeam
            _teamServiceMock.Setup(r => r.DeleteTeam(It.Is<Guid>(i => i == _mockTeamId2)))
                .Callback(new Action<Guid>(x =>
                {
                    for (int i = 0; i < _teamListMock.Count; i++)
                    {
                        if (_teamListMock[i].TeamID == x)
                        {
                            _teamListMock[i].DeletedAt = DateTime.Now.Date;
                            index = i;
                        }
                    }
                }));
        }

        [Test]
        public async Task Calling_GetTeam_Returns_OK_With_Team_As_Team()
        {
            // Arrange
            TeamController mockTeamController = new TeamController(mockLogger.Object, _teamServiceMock.Object, mockTokenService.Object, mockMailService.Object);
            var qc = new QueryCollection(new Dictionary<string, StringValues> { { "Id", new StringValues(_mockTeamId1.ToString()) } });
            _mockRequest = new Mock<HttpRequest>();
            _mockRequest.Setup(x => x.Query).Returns(() => qc);

            // Act
            var result = await mockTeamController.GetTeam(_mockRequest.Object);
            var OkResult = result as OkObjectResult;
            Model.Entity.Team resultTeam = (Model.Entity.Team)OkResult.Value;
            // Assert
            Assert.IsNotNull(result);
            Assert.IsNotNull(OkResult);
            Assert.AreEqual(resultTeam.TeamID, _teamListMock[0].TeamID);
            Assert.AreEqual(200, OkResult.StatusCode);
            _teamServiceMock.Verify(c => c.GetTeam(_mockTeamId1), Times.Once);
        }

        /*[Test]
        public async Task Calling_CreateTeam_Return_OK_With_TeamName_And_TeamId()
        {
            // Arrange
            TeamController mockTeamController = new TeamController(mockLogger.Object, _teamServiceMock.Object, mockTokenService.Object, mockMailService.Object);
            Guid mockId = Guid.NewGuid();
            Model.DTO.TeamBaseDTO dto = new Model.DTO.TeamBaseDTO()
            {
                OrganizationId = _mockOrganizationId,
                Name = "latest mock team"
            };
            _mockRequest = CreateMockRequest(dto);

            // Act
            var result = await mockTeamController.CreateTeam(_mockRequest.Object);
            var OkResult = result as OkObjectResult;
            string resultValue = (string)OkResult.Value;
            // Assert

            Assert.IsNotNull(result);
            Assert.IsNotNull(OkResult);
            Assert.AreEqual(OkResult.Value, resultValue);
            Assert.AreEqual(200, OkResult.StatusCode);
            _teamServiceMock.Verify(c => c.CreateTeam(It.IsAny<TeamBaseDTO>()), Times.Once);
            _memoryStream.Dispose();
        }*/

        [Test]
        public async Task Calling_UpdateTeam_Returns_OK_And_Updates_Team()
        {
            // Arrange
            TeamController mockTeamController = new TeamController(mockLogger.Object, _teamServiceMock.Object, mockTokenService.Object, mockMailService.Object);
            Model.DTO.TeamBaseWithIdDTO dto = new Model.DTO.TeamBaseWithIdDTO()
            {
                OrganizationId = _mockOrganizationId,
                TeamId = _mockTeamId1,
                Name = "Updated mock team",
            };

            _mockRequest = CreateMockRequest(dto);

            // Act
            var result = await mockTeamController.UpdateTeam(_mockRequest.Object);
            var OkResult = result as OkObjectResult;
            string resultValue = (string)OkResult.Value;

            // Assert            
            Assert.IsNotNull(result);
            Assert.IsNotNull(OkResult);
            Assert.AreEqual(OkResult.Value, resultValue);
            Assert.AreEqual(200, OkResult.StatusCode);
            _teamServiceMock.Verify(c => c.UpdateTeam(It.IsAny<TeamBaseWithIdDTO>()), Times.Once);
        }

        [Test]
        public async Task Calling_DeleteTeam_Returns_OK_And_Sets_DeletedAt_Property_To_DateTimeNow()
        {
            // Arrange
            TeamController mockTeamController = new TeamController(mockLogger.Object, _teamServiceMock.Object, mockTokenService.Object, mockMailService.Object);
            var qc = new QueryCollection(new Dictionary<string, StringValues> { { "Id", new StringValues(_mockTeamId2.ToString()) } });
            _mockRequest = new Mock<HttpRequest>();
            _mockRequest.Setup(x => x.Query).Returns(() => qc);

            // Act
            var result = await mockTeamController.DeleteTeam(_mockRequest.Object);
            var OkResult = result as OkObjectResult;
            var resultValue = (string)OkResult.Value;

            // Assert
            _teamServiceMock.Verify(c => c.DeleteTeam(_mockTeamId2), Times.Once);
            Assert.IsNotNull(result);
            Assert.IsNotNull(OkResult);
            Assert.AreEqual(OkResult.Value, resultValue);
            Assert.AreEqual(200, OkResult.StatusCode);
        }

        [TearDown]
        public void CleanUp()
        {
            _teamServiceMock = null;
            mockLogger = null;
            _mockRequest = null;
            _teamListMock = null;
            _teamBaseListMock = null;

        }

    }
}

