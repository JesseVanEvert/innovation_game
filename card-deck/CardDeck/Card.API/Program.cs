using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using CardDeck.BLL.Interfaces;
using CardDeck.BLL;
using CardDeck.DAL;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using System.IO;
using CardDeck.DAL.Repository;
using CardDeck.DAL.RepositoryInterface;
using System;

[assembly: FunctionsStartup(typeof(Card.API.Startup))]
namespace Card.API
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            builder.Services.AddScoped<ICardService, CardService>();
            builder.Services.AddScoped<ICardRepository, CardRepository>();
            builder.Services.AddScoped<ITokenService, TokenService>();


#if DEBUG
            var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("local.settings.json", true, true)
            .Build();

            builder.Services.AddDbContext<CardContext>(options =>
                options.UseSqlServer(configuration["SqlConnectionString"]));
#else
            builder.Services.AddDbContext<CardContext>(options =>
                options.UseSqlServer(Environment.GetEnvironmentVariable("SqlConnectionString")));
#endif
        }
    }
}
