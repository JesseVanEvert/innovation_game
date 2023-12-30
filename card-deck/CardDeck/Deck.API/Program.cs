using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using CardDeck.BLL.Interfaces;
using CardDeck.BLL;
using Microsoft.Extensions.Configuration;
using System.IO;
using CardDeck.DAL;
using Microsoft.EntityFrameworkCore;
using CardDeck.DAL.RepositoryInterface;
using CardDeck.DAL.Repository;
using System;

[assembly: FunctionsStartup(typeof(Deck.API.Startup))]
namespace Deck.API
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            builder.Services.AddScoped<IDeckService, DeckService>();
            builder.Services.AddScoped<IDeckRepository, DeckRepository>();
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


