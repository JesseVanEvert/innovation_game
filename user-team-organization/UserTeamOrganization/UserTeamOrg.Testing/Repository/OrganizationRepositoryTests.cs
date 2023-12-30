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
using UserTeamOrg.Model.Entity;
using UserTeamOrg.DAL.Interfaces;

namespace UserTeamOrg.Testing.Repository
{
    public class OrganizationRepositoryTests
    {
        private Mock<IOrganizationRepository> _organizationRepoMock;
        private IOrganizationRepository _organizationRepo;
        private List<Model.Entity.Organization> _organizationListMock;

        private int index;
        private Guid _organizationIdMock1 = Guid.NewGuid();
        private Guid _organizationIdMock2 = Guid.NewGuid();
        private Guid newOrganizationId = Guid.NewGuid();

        [SetUp]
        public void Setup()
        {
            _organizationRepoMock = new Mock<IOrganizationRepository>();


            Model.Entity.Organization mockOrganization1 = new Model.Entity.Organization()
            {
                OrganizationID = _organizationIdMock1,
                Name = "mockOrganization1",
                ImageUrl = null,
                DeletedAt = null
            };
            Model.Entity.Organization mockOrganization2 = new Model.Entity.Organization()
            {
                OrganizationID = _organizationIdMock2,
                Name = "mockOrganization2",
                ImageUrl = null,
                DeletedAt = null
            };
            _organizationListMock = new List<Model.Entity.Organization>();
            _organizationListMock.Add(mockOrganization1);
            _organizationListMock.Add(mockOrganization2);


            // GetTeam
            _organizationRepoMock.Setup(m => m.GetOrganization(It.Is<Guid>(i => i == _organizationIdMock1)))
                .ReturnsAsync(_organizationListMock[0]);

            // RegisterUser
            _organizationRepoMock.Setup(m => m.CreateOrganization(It.IsAny<Model.Entity.Organization>()))
                .Callback(new Action<Model.Entity.Organization>(x =>
                {
                    _organizationListMock.Add(x);
                }));

            // UpdateUser
            _organizationRepoMock.Setup(m => m.UpdateOrganization(It.IsAny<Model.Entity.Organization>()))
                .Callback(new Action<Model.Entity.Organization>(x =>
                {
                    for (int i = 0; i < _organizationListMock.Count(); i++)
                    {
                        if (_organizationListMock[i].OrganizationID == x.OrganizationID)
                        {
                            _organizationListMock[i] = x;
                            index = i;
                        }
                    }
                }));

            // DeleteUser
            _organizationRepoMock.Setup(m => m.DeleteOrganization(It.Is<Guid>(i => i == _organizationIdMock1)))
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

            _organizationRepo = _organizationRepoMock.Object;
        }

        [Test]
        public async Task Calling_GetOrganization_Returns_Correct_Organization_As_Organization()
        {
            // Arrange

            // Act
            Model.Entity.Organization? result = await _organizationRepo.GetOrganization(_organizationIdMock1);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(result.OrganizationID, _organizationListMock[0].OrganizationID);
            Assert.That(result, Is.InstanceOf(typeof(Model.Entity.Organization)));
        }

        [Test]
        public async Task Calling_CreateOrganization_Adds_Organization()
        {
            // Arrange
            Model.Entity.Organization createOrganization = new Model.Entity.Organization()
            {
                OrganizationID = newOrganizationId,
                Name = "latest mock Organization",
                ImageUrl = null,
                DeletedAt = null
            };

            // Act
            await _organizationRepo.CreateOrganization(createOrganization);

            // Assert                        
            Assert.AreEqual(createOrganization.Name, _organizationListMock[2].Name);

        }

        [Test]
        public async Task Calling_UpdateOrganization_Changes_Organization_information_R()
        {
            // Arrange            
            Model.Entity.Organization updateOrganization = new Model.Entity.Organization()
            {
                OrganizationID = _organizationIdMock1,
                Name = "updated mock Organization",
                DeletedAt = null,
                ImageUrl = "angrycats.com/surprisedcat.jpg",
            };

            // Act
            await _organizationRepo.UpdateOrganization(updateOrganization);

            // Assert                        
            Assert.AreEqual(updateOrganization.Name, _organizationListMock[index].Name);
        }

        [Test]
        public async Task Calling_DeleteOrganization_Changes_DeletedAt_Property_To_Current_DateTime()
        {
            // Arrange

            // Act
            await _organizationRepo.DeleteOrganization(_organizationIdMock1);

            // Assert
            Assert.AreEqual(_organizationListMock[index].DeletedAt, DateTime.Now.Date);
        }


        [TearDown]
        public void TestCleanUp()
        {
            _organizationRepo = null;
            _organizationRepoMock = null;
            _organizationListMock = null;
            index = 33;
        }
    }
}

