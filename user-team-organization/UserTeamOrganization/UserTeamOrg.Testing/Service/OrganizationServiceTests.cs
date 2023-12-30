using UserTeamOrg.DAL.Interfaces;
using UserTeamOrg.DAL;
using UserTeamOrg.Model;
using UserTeamOrg.Exceptions;
using Moq;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using UserTeamOrg.BLL;
using Microsoft.AspNetCore.Mvc;

namespace UserTeamOrg.Testing.Service
{
    public class OrganizationServiceTests
    {
        private EnvironmentVariables env = new EnvironmentVariables();
        private Mock<IOrganizationRepository> _organizationRepoMock;
        private OrganizationService _organizationServiceMock;
        private List<Model.Entity.Organization> _organizationListMock;

        private int index;
        private Guid mockOrganizationId1 = Guid.NewGuid();
        private Guid mockOrganizationId2 = Guid.NewGuid();        

        [SetUp]
        public void Setup()
        {
            // Set mock environment variables
            env.SetMockVariables();

            // Set up mock service
            _organizationRepoMock = new Mock<IOrganizationRepository>();
            _organizationServiceMock= new OrganizationService(_organizationRepoMock.Object);
            _organizationListMock = new List<Model.Entity.Organization>();

            // Set up mock Organizations
            Model.Entity.Organization org1 = new Model.Entity.Organization()
            {
                OrganizationID = mockOrganizationId1,
                Name = "Mock organization 1",
                ImageUrl = null,
                DeletedAt = null
            };
            Model.Entity.Organization org2 = new Model.Entity.Organization()
            {
                OrganizationID = mockOrganizationId2,
                Name = "Mock organization 2",
                ImageUrl = null,
                DeletedAt = null
            };

            _organizationListMock.Add(org1);
            _organizationListMock.Add(org2);

            //GetOrganization
            _organizationRepoMock.Setup(m => m.GetOrganization(mockOrganizationId1)).ReturnsAsync(_organizationListMock[0]);

            //CreateOrganization
            _organizationRepoMock.Setup(m => m.CreateOrganization(It.IsAny<Model.Entity.Organization>()))
                .Callback(new Action<Model.Entity.Organization>(x =>
                {
                    _organizationListMock.Add(x);
                }));

            //UpdateOrganization
            _organizationRepoMock.Setup(m => m.UpdateOrganization(It.IsAny<Model.Entity.Organization>()))
                .Callback(new Action<Model.Entity.Organization>(x =>
                {
                    for (int i = 0; i < _organizationListMock.Count(); i++)
                    {
                        // Find the mock Organization in the list to update
                        if (_organizationListMock[i].OrganizationID == x.OrganizationID)
                        {
                            _organizationListMock[i] = x;
                            // check if index will be properly updated
                            index = i;
                        }
                    }
                }));

            //DeleteOrganization
            _organizationRepoMock.Setup(m => m.DeleteOrganization(mockOrganizationId1))
                .Callback(new Action<Guid>(x =>
                {
                    for (int i = 0; i < _organizationListMock.Count(); i++)
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
        public async Task Calling_GetOrganization_Returns_Correct_Organization_As_Organization()
        {
            // Arrange

            // Act
            Model.Entity.Organization result = await _organizationServiceMock.GetOrganization(mockOrganizationId1);

            // Assert
            Assert.AreEqual(result.Name, _organizationListMock[0].Name);
            Assert.That(result, Is.InstanceOf(typeof(Model.Entity.Organization)));

            _organizationRepoMock.Verify(c => c.GetOrganization(mockOrganizationId1), Times.Once);
        }

        [Test]
        public async Task Calling_CreateOrganization_Adds_Organization_And_Returns_Organization()
        {
            // Arrange
            // new Mock Organization to create
            string mockName = "New Mock Organization";
            Guid mockOrganizationId = Guid.NewGuid();
            Model.DTO.OrganizationBaseDTO dto = new Model.DTO.OrganizationBaseDTO()
            {
                Name = mockName
            };            
            
            // Act
            Model.Entity.Organization result = await _organizationServiceMock.CreateOrganization(dto);
            // Assert            
            _organizationRepoMock.Verify(m => m.CreateOrganization(It.IsAny<Model.Entity.Organization>()), Times.Once);
            Assert.AreEqual(result.Name, _organizationListMock[2].Name);
            Assert.That(result, Is.InstanceOf(typeof(Model.Entity.Organization)));

        }

        [Test]
        public async Task Calling_UpdateOrganization_Changes_Organization_information()
        {
            // Arrange
            // Mock Organization to update existing Organization
            string mockName = "updateMock";
            Model.DTO.OrganizationBaseWithIdDTO dto = new Model.DTO.OrganizationBaseWithIdDTO()
            {
                Name = mockName,
                OrganizationId = mockOrganizationId1
            };


            // Act
            await _organizationServiceMock.UpdateOrganization(dto);

            // Assert
            Assert.AreEqual(dto.Name, _organizationListMock[index].Name);
            _organizationRepoMock.Verify(c => c.UpdateOrganization(It.IsAny<Model.Entity.Organization>()), Times.Once);
        }

        [Test]
        public async Task Calling_DeleteOrganization_Changes_DeletedAt_Property_To_Current_DateTime()
        {            
            // Arrange

            // Act
            await _organizationServiceMock.DeleteOrganization(mockOrganizationId1);

            // Assert
            Assert.AreEqual(_organizationListMock[index].DeletedAt, DateTime.Now.Date);
            _organizationRepoMock.Verify(c => c.DeleteOrganization(mockOrganizationId1), Times.Once);
        }

        [TearDown]
        public void TestCleanUp()
        {
            _organizationRepoMock = null;
            _organizationServiceMock = null;
            _organizationListMock = null;
            index = 33;
        }
    }
}

