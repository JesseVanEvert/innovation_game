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
using UserTeamOrg.DAL.Interfaces;

namespace UserTeamOrg.Testing.Repository
{
    public class TeamRepositoryTests
    {
        private Mock<ITeamRepository> _teamRepoMock;
        private ITeamRepository _teamRepo;
        private List<Model.Entity.Team> _teamListMock;

        private int index;
        private Guid _teamIdMock1 = Guid.NewGuid();
        private Guid _teamIdMock2 = Guid.NewGuid();
        private Guid newTeamId = Guid.NewGuid();
        private Guid _organizationIdMock1 = Guid.NewGuid();
        private Guid _organizationIdMock2 = Guid.NewGuid();


        [SetUp]
        public void Setup()
        {
            _teamRepoMock = new Mock<ITeamRepository>();


            Model.Entity.Team mockteam1 = new Model.Entity.Team()
            {
                TeamID = _teamIdMock1,
                OrganizationID = _organizationIdMock1,
                Name = "mockTeam1",
                DeletedAt = null
            };
            Model.Entity.Team mockteam2 = new Model.Entity.Team()
            {
                TeamID = _teamIdMock2,
                OrganizationID = _organizationIdMock2,
                Name = "mockTeam2",
                DeletedAt = null
            };
            _teamListMock = new List<Model.Entity.Team>();
            _teamListMock.Add(mockteam1);
            _teamListMock.Add(mockteam2);


            // GetTeam
            _teamRepoMock.Setup(m => m.GetTeam(It.Is<Guid>(i => i == _teamIdMock1)))
                .ReturnsAsync(_teamListMock[0]);

            // RegisterUser
            _teamRepoMock.Setup(m => m.CreateTeam(It.IsAny<Model.Entity.Team>()))
                .Callback(new Action<Model.Entity.Team>(x =>
                {
                    _teamListMock.Add(x);
                }));

            // UpdateUser
            _teamRepoMock.Setup(m => m.UpdateTeam(It.IsAny<Model.Entity.Team>()))
                .Callback(new Action<Model.Entity.Team>(x =>
                {
                    for (int i = 0; i < _teamListMock.Count(); i++)
                    {
                        if (_teamListMock[i].TeamID == x.TeamID)
                        {
                            _teamListMock[i] = x;
                            index = i;
                        }
                    }
                }));

            // DeleteUser
            _teamRepoMock.Setup(m => m.DeleteTeam(It.Is<Guid>(i => i == _teamIdMock1)))
                .Callback(new Action<Guid>(x =>
                {
                    for (int i = 0; i < _teamListMock.Count(); i++)
                    {
                        if (_teamListMock[i].TeamID == x)
                        {
                            _teamListMock[i].DeletedAt = DateTime.Now.Date;
                            index = i;
                        }
                    }
                }));

            _teamRepo = _teamRepoMock.Object;
        }

        [Test]
        public async Task Calling_GetTeam_Returns_Correct_Team_As_Team()
        {
            // Arrange

            // Act
            Model.Entity.Team? result = await _teamRepo.GetTeam(_teamIdMock1);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(result.TeamID, _teamListMock[0].TeamID);
            Assert.That(result, Is.InstanceOf(typeof(Model.Entity.Team)));
        }

        [Test]
        public async Task Calling_RegisterTeam_Adds_Team()
        {
            // Arrange
            Model.Entity.Team createTeam = new Model.Entity.Team()
            {
                TeamID = newTeamId,
                OrganizationID = Guid.NewGuid(),
                Name = "latest mock team",
                DeletedAt = null
            };

            // Act
            await _teamRepo.CreateTeam(createTeam);

            // Assert                        
            Assert.AreEqual(createTeam.Name, _teamListMock[2].Name);

        }

        [Test]
        public async Task Calling_UpdateTeam_Changes_Team_information_R()
        {
            // Arrange            
            Model.Entity.Team updateTeam = new Model.Entity.Team()
            {
                TeamID = _teamIdMock1,
                OrganizationID = _organizationIdMock1,
                Name = "updated mock team",
                DeletedAt = null
            };

            // Act
            await _teamRepo.UpdateTeam(updateTeam);

            // Assert                        
            Assert.AreEqual(updateTeam.Name, _teamListMock[index].Name);

        }

        [Test]
        public async Task Calling_DeleteTeam_Changes_DeletedAt_Property_To_Current_DateTime()
        {
            // Arrange

            // Act
            await _teamRepo.DeleteTeam(_teamIdMock1);

            // Assert
            Assert.AreEqual(_teamListMock[index].DeletedAt, DateTime.Now.Date);
        }


        [TearDown]
        public void TestCleanUp()
        {
            _teamRepo = null;
            _teamRepoMock = null;
            _teamListMock = null;
            index = 33;
        }

    }
}

