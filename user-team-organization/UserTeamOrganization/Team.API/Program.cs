using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using UserTeamOrg.BLL.Interfaces;
using UserTeamOrg.BLL;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.IO;
using UserTeamOrg.DAL.Interfaces;
using UserTeamOrg.DAL.Repository;
using UserTeamOrg.DAL;
using System;

[assembly: FunctionsStartup(typeof(Team.API.Startup))]
namespace Team.API
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            builder.Services.AddScoped<ITeamService, TeamService>();
            builder.Services.AddScoped<ITeamRepository, TeamRepository>();
            builder.Services.AddScoped<ITokenService, TokenService>();

            builder.Services.AddScoped<IMailService, MailService>();

#if DEBUG
            var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("local.settings.json", true, true)
            .Build();

            builder.Services.AddDbContext<UserTeamOrganizationContext>(options =>
                options.UseSqlServer(configuration["SqlConnectionString"]));
#else
            builder.Services.AddDbContext<UserTeamOrganizationContext>(options =>
                options.UseSqlServer(Environment.GetEnvironmentVariable("SqlConnectionString")));
#endif
        }
    }
}