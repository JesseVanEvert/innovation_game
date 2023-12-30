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
    public class TeamServiceTests
    {
        private EnvironmentVariables env = new EnvironmentVariables();
        private Mock<ITeamRepository> _teamRepoMock;
        private TeamService _teamServiceMock;
        private List<Model.Entity.Team> _teamListMock;

        private int index;
        private Guid mockOrganizationId1 = Guid.NewGuid();
        private Guid mockOrganizationId2 = Guid.NewGuid();
        private Guid mockTeamId1 = Guid.NewGuid();
        private Guid mockTeamId2 = Guid.NewGuid();

        [SetUp]
        public void Setup()
        {
            // Set mock environment variables
            env.SetMockVariables();

            // Set up mock service
            _teamRepoMock = new Mock<ITeamRepository>();
            _teamServiceMock = new TeamService(_teamRepoMock.Object);


            // Set up mock Teams
            Model.Entity.Team team1 = new Model.Entity.Team()
            {
                TeamID = mockTeamId1,
                OrganizationID = mockOrganizationId1,
                Name = "MockTeam1",
                ImageUrl = null,
                DeletedAt = null,
            };

            Model.Entity.Team team2 = new Model.Entity.Team()
            {
                TeamID = mockTeamId2,
                OrganizationID = mockOrganizationId2,
                Name = "MockTeam2",
                ImageUrl = null,
                DeletedAt = null,
            };
            _teamListMock = new List<Model.Entity.Team>();
            _teamListMock.Add(team1);
            _teamListMock.Add(team2);

            // GetTeam
            _teamRepoMock.Setup(m => m.GetTeam(mockTeamId1)).ReturnsAsync(_teamListMock[0]);

            // CreateTeam
            _teamRepoMock.Setup(m => m.CreateTeam(It.IsAny<Model.Entity.Team>()))
                .Callback(new Action<Model.Entity.Team>(x =>
                {
                    _teamListMock.Add(x);
                }));

            // UpdateTeam
            _teamRepoMock.Setup(m => m.UpdateTeam(It.IsAny<Model.Entity.Team>()))
                .Callback(new Action<Model.Entity.Team>(x =>
                {
                    for (int i = 0; i < _teamListMock.Count(); i++)
                    {
                        // Find the mock Team in the list to update
                        if (_teamListMock[i].TeamID == x.TeamID)
                        {
                            _teamListMock[i] = x;
                            // check if index will be properly updated
                            index = i;
                        }
                    }
                }));

            // DeleteTeam
            _teamRepoMock.Setup(m => m.DeleteTeam(mockTeamId1)).Callback(new Action<Guid>(x =>
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
        }

        [Test]
        public async Task Calling_GetTeam_Returns_Correct_Team_As_Team()
        {
            // Arrange

            // Act
            Model.Entity.Team result = await _teamServiceMock.GetTeam(mockTeamId1);

            // Assert
            Assert.AreEqual(result.Name, _teamListMock[0].Name);
            Assert.That(result, Is.InstanceOf(typeof(Model.Entity.Team)));

            _teamRepoMock.Verify(c => c.GetTeam(mockTeamId1), Times.Once);
        }

        [Test]
        public async Task Calling_CreateTeam_Adds_Team_And_Returns_Team()
        {
            // Arrange            
            // new Mock team to create
            string mockName = "New Mock Team";
            Guid mockOrganizationId = Guid.NewGuid();
            Model.DTO.TeamBaseDTO dto = new Model.DTO.TeamBaseDTO()
            {
                Name = mockName,
                OrganizationId = mockOrganizationId
            };

            // Act
            Model.Entity.Team result = await _teamServiceMock.CreateTeam(dto);
            // Assert            
            Assert.AreEqual(result.Name, _teamListMock[2].Name);
            Assert.That(result, Is.InstanceOf(typeof(Model.Entity.Team)));
            _teamRepoMock.Verify(m => m.CreateTeam(It.IsAny<Model.Entity.Team>()), Times.Once);
        }

        [Test]
        public async Task Calling_UpdateTeam_Changes_Team_information()
        {
            // Arrange
            // Mock team to update teamlist
            string mockName = "updateMock";
            Model.DTO.TeamBaseWithIdDTO dto = new Model.DTO.TeamBaseWithIdDTO()
            {
                Name = mockName,
                TeamId = mockTeamId2,
                OrganizationId = mockOrganizationId2
            };

            // Act
            await _teamServiceMock.UpdateTeam(dto);

            // Assert
            Assert.AreEqual(dto.Name, _teamListMock[index].Name);
            _teamRepoMock.Verify(c => c.UpdateTeam(It.IsAny<Model.Entity.Team>()), Times.Once);

        }

        [Test]
        public async Task Calling_DeleteTeam_Changes_DeletedAt_Property_To_Current_DateTime()
        {

            // Arrange

            // Act
            await _teamServiceMock.DeleteTeam(mockTeamId1);

            // Assert
            Assert.AreEqual(_teamListMock[index].DeletedAt, DateTime.Now.Date);
            _teamRepoMock.Verify(c => c.DeleteTeam(mockTeamId1), Times.Once);

        }

        [TearDown]
        public void TestCleanUp()
        {
            _teamRepoMock = null;
            _teamServiceMock = null;
            _teamListMock = null;
            index = 33;
        }

    }


}

