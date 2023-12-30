using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using UserTeamOrg.BLL.Interfaces;
using UserTeamOrg.BLL;
using UserTeamOrg.DAL.Repository;
using UserTeamOrg.DAL.Interfaces;
using Microsoft.EntityFrameworkCore;
using UserTeamOrg.DAL;
using Microsoft.Extensions.Configuration;
using System.IO;
using System;

[assembly: FunctionsStartup(typeof(Organization.API.Startup))]
namespace Organization.API
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            builder.Services.AddScoped<IOrganizationService, OrganizationService>();
            builder.Services.AddScoped<IOrganizationRepository, OrganizationRepository>();
            builder.Services.AddScoped<ITokenService, TokenService>();

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