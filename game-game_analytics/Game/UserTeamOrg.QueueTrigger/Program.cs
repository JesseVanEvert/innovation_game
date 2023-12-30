using Microsoft.Extensions.DependencyInjection;
using Game.BLL.Interfaces;
using Game.BLL;
using Microsoft.Extensions.Configuration;
using System.IO;
using Game.DAL;
using Microsoft.EntityFrameworkCore;
using Game.DAL.Interfaces;
using Game.DAL.Repository;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Game.BLL.Service;
using UserTeamOrg.DAL.Repository;
using System;

[assembly: FunctionsStartup(typeof(UserTeamOrg.QueueTrigger.Startup))]
namespace UserTeamOrg.QueueTrigger
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            builder.Services.AddScoped<IOrganizationRepository, OrganizationRepository>();
            builder.Services.AddScoped<ITeamRepository, TeamRepository>();
            builder.Services.AddScoped<IUserRepository, UserRepository>();
            builder.Services.AddScoped<IUserService, UserService>();
            builder.Services.AddScoped<ITeamService, TeamService>();
            builder.Services.AddScoped<IOrganizationService, OrganizationService>();
            builder.Services.AddScoped<IGameService, GameService>();
            builder.Services.AddScoped<IUserService, UserService>();

#if DEBUG
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("local.settings.json", true, true)
                .Build();
            builder.Services.AddDbContext<GameContext>(options =>
                options.UseSqlServer(configuration["SqlConnectionString"]));
#else
            builder.Services.AddDbContext<GameContext>(options =>
                options.UseSqlServer(Environment.GetEnvironmentVariable("SqlConnectionString")));
#endif
        }
    }
}


