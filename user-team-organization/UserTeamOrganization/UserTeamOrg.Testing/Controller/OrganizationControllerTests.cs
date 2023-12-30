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
using Organization.API.Controllers;
using UserTeamOrg.Model.Entity;


namespace UserTeamOrg.Testing.Controller

{
    public class OrganizationControllerTests
    {
        private Mock<ITokenService> mockTokenService;
        private Mock<IOrganizationService> _organizationServiceMock;
        private Mock<ILogger<OrganizationController>> mockLogger;
        private List<Model.Entity.Organization> _organizationListMock;
        private List<OrganizationBaseWithIdDTO> _organizationBaseListMock;
        private Dictionary<string, string> mockAuthResponse;

        // Generate Guids here so we can check them later
        private Guid _organizationId1 = Guid.NewGuid();
        private Guid _organizationId2 = Guid.NewGuid();
        private Guid _newOrganizationId = Guid.NewGuid();        
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
            mockRequest.Setup(x => x.Headers);
            mockRequest.Setup(x => x.Body).Returns(_memoryStream);

            return mockRequest;
        }


        [SetUp]
        public void Setup()
        {
            mockAuthResponse = new Dictionary<string, string>();
            mockTokenService = new Mock<ITokenService>();
            
            _organizationServiceMock = new Mock<IOrganizationService>();
            mockLogger = new Mock<ILogger<OrganizationController>>();

            _organizationListMock = new List<Model.Entity.Organization>();
            Model.Entity.Organization organization1 = new Model.Entity.Organization()
            {
                OrganizationID = _organizationId1,
                Name = "mock organization 1",
                DeletedAt = null,
                ImageUrl = null,
            };
            Model.Entity.Organization organization2 = new Model.Entity.Organization()
            {
                OrganizationID = _organizationId2,
                Name = "mock organization 2",
                DeletedAt = null,
                ImageUrl = null,
            };

            _organizationListMock.Add(organization1);
            _organizationListMock.Add(organization2);

            OrganizationBaseDTO createOrganizationDTO = new OrganizationBaseDTO()
            {
                Name = "newest mock organization",                               
            };

            OrganizationBaseWithIdDTO updateOrganizationDTO = new OrganizationBaseWithIdDTO()
            {
                Name = "updated mock organization",
                OrganizationId = _organizationId1
            };

            Model.Entity.Organization newOrganization = new Model.Entity.Organization()
            {
                Name = "newest mock organization",
                OrganizationID = _newOrganizationId
            };

            mockTokenService.Setup(fu => fu.GetRequestAuth(It.IsAny<HttpRequest>()))
                .ReturnsAsync(mockAuthResponse);

            mockTokenService.Setup(fu => fu.CheckAccessLevel(It.IsAny<Dictionary<string, string>>(), It.IsAny<Model.Entity.AccessLevel>()))
                .Returns(true);

            // GetOrganization
            _organizationServiceMock.Setup(r => r.GetOrganization(It.Is<Guid>(i => i == _organizationId1)))
                .ReturnsAsync(_organizationListMock[0]);

            // CreateOrganization
            _organizationServiceMock.Setup(r => r.CreateOrganization(It.IsAny<OrganizationBaseDTO>()))
                .ReturnsAsync(newOrganization)
                .Callback(new Action<OrganizationBaseDTO>(x =>
                {                    
                    newOrganization.Name = x.Name;
                    _organizationListMock.Add(newOrganization);
                }));

            // UpdateOrganization
            _organizationServiceMock.Setup(r => r.UpdateOrganization(It.IsAny<OrganizationBaseWithIdDTO>()))
                .Callback(new Action<OrganizationBaseWithIdDTO>(x =>
                {
                    for (int i = 0; i < _organizationListMock.Count; i++)
                    {
                        if (_organizationListMock[i].OrganizationID == x.OrganizationId)
                        {
                            _organizationListMock[i].Name = x.Name;                            
                            index = i;
                        }
                    }
                }));

            // DeleteOrganization
            _organizationServiceMock.Setup(r => r.DeleteOrganization(It.Is<Guid>(i => i == _organizationId2)))
                .Callback(new Action<Guid>(x =>
                {
                    for (int i = 0; i < _organizationListMock.Count; i++)
                    {
                        if (_organizationListMock[i].OrganizationID == x)
                        {
                            _organizationListMock[i].DeletedAt = DateTime.Now.Date;
                            index = i;
                        }
                    }
                }));
        }

        [Test]
        public async Task Calling_GetOrganization_Returns_OK_With_Organization_As_Organization()
        {
            // Arrange
            OrganizationController mockOrganizationController = new OrganizationController(mockLogger.Object, _organizationServiceMock.Object, mockTokenService.Object);
            var qc = new QueryCollection(new Dictionary<string, StringValues> { { "Id", new StringValues(_organizationId1.ToString()) } });
            _mockRequest = new Mock<HttpRequest>();            
            _mockRequest.Setup(x => x.Query).Returns(() => qc);

            // Act
            var result = await mockOrganizationController.GetOrganization(_mockRequest.Object);
            var OkResult = result as OkObjectResult;
            Model.Entity.Organization resultOrganization = (Model.Entity.Organization)OkResult.Value;
            // Assert
            Assert.IsNotNull(result);
            Assert.IsNotNull(OkResult);
            Assert.AreEqual(resultOrganization.OrganizationID, _organizationListMock[0].OrganizationID);
            Assert.AreEqual(200, OkResult.StatusCode);
            _organizationServiceMock.Verify(c => c.GetOrganization(_organizationId1), Times.Once);
        }

        /*[Test]
        public async Task Calling_CreateOrganization_Return_OK_With_OrganizationName_And_OrganizationId()
        {
            // Arrange
            OrganizationController mockOrganizationController = new OrganizationController(mockLogger.Object, _organizationServiceMock.Object, mockTokenService.Object);            
            Model.DTO.OrganizationBaseDTO dto = new Model.DTO.OrganizationBaseDTO()
            {                
                Name = "newest mock organization"
            };
            _mockRequest = CreateMockRequest(dto);

            // Act
            var result = await mockOrganizationController.CreateOrganization(_mockRequest.Object);
            var OkResult = result as OkObjectResult;
            string resultValue = (string)OkResult.Value;
            // Assert
            Assert.IsNotNull(result);
            Assert.IsNotNull(OkResult);
            Assert.AreEqual(OkResult.Value, resultValue);
            Assert.AreEqual(200, OkResult.StatusCode);
            _organizationServiceMock.Verify(c => c.CreateOrganization(It.IsAny<OrganizationBaseDTO>()));
            _memoryStream.Dispose();
        }*/

        [Test]
        public async Task Calling_UpdateOrganization_Returns_OK_And_Updates_Organization()
        {
            // Arrange
            OrganizationController mockOrganizationController = new OrganizationController(mockLogger.Object, _organizationServiceMock.Object, mockTokenService.Object);
            Model.DTO.OrganizationBaseWithIdDTO dto = new Model.DTO.OrganizationBaseWithIdDTO()
            {
                OrganizationId = _organizationId1,
                Name = "Updated mock organization",
            };
            _mockRequest = CreateMockRequest(dto);

            // Act
            var result = await mockOrganizationController.UpdateOrganization(_mockRequest.Object);
            var OkResult = result as OkObjectResult;
            string resultValue = (string)OkResult.Value;

            // Assert            
            Assert.IsNotNull(result);
            Assert.IsNotNull(OkResult);
            Assert.AreEqual(OkResult.Value, resultValue);
            Assert.AreEqual(200, OkResult.StatusCode);
            _organizationServiceMock.Verify(c => c.UpdateOrganization(It.IsAny<OrganizationBaseWithIdDTO>()), Times.Once);
        }

        [Test]
        public async Task Calling_DeleteUser_Returns_OK_And_Sets_DeletedAt_Property_To_DateTimeNow()
        {
            // Arrange
            OrganizationController mockOrganizationController = new OrganizationController(mockLogger.Object, _organizationServiceMock.Object, mockTokenService.Object);
            var qc = new QueryCollection(new Dictionary<string, StringValues> { { "Id", new StringValues(_organizationId2.ToString()) } });
            _mockRequest = new Mock<HttpRequest>();
            _mockRequest.Setup(x => x.Query).Returns(() => qc);

            // Act
            var result = await mockOrganizationController.DeleteOrganization(_mockRequest.Object);
            var OkResult = result as OkObjectResult;
            var resultValue = (string)OkResult.Value;

            // Assert
            _organizationServiceMock.Verify(c => c.DeleteOrganization(_organizationId2), Times.Once);
            Assert.IsNotNull(result);
            Assert.IsNotNull(OkResult);
            Assert.AreEqual(OkResult.Value, resultValue);
            Assert.AreEqual(200, OkResult.StatusCode);
        }

        [TearDown]
        public void CleanUp()
        {
            _organizationServiceMock = null;
            mockLogger = null;
            _mockRequest = null;
            _organizationListMock = null;
            _organizationBaseListMock = null;

        }

    }
}

