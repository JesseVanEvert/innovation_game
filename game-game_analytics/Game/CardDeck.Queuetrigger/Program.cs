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
using System;

[assembly: FunctionsStartup(typeof(CardDeck.Queuetrigger.Startup))]
namespace CardDeck.Queuetrigger
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        { 
            builder.Services.AddScoped<IDeckService, DeckService>();
            builder.Services.AddScoped<IDeckRepository, DeckRepository>();
            builder.Services.AddScoped<ICardService, CardService>();
            builder.Services.AddScoped<ICardRepository, CardRepository>();

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


