using Game.BLL;
using Game.BLL.Interfaces;
using Game.DAL;
using Game.DAL.Interfaces;
using Game.DAL.Repository;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.IO;

[assembly: FunctionsStartup(typeof(Game.API.Startup))]
namespace Game.API
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            builder.Services.AddScoped<IGameService, GameService>();
            builder.Services.AddScoped<IUserService, UserService>();
            builder.Services.AddScoped<IDeckRepository, DeckRepository>();
            builder.Services.AddScoped<IGameRepository, GameRepository>();
            builder.Services.AddScoped<IUserRepository, UserRepository>();       
            builder.Services.AddScoped<ITokenService, TokenService>();
#if DEBUG
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("local.settings.json", true, true)
                .Build();

            builder.Services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = configuration["RedisConnectionString"];
                options.InstanceName = "InnovationGame_";
            });

            builder.Services.AddDbContext<GameContext>(options =>
                options.UseSqlServer(configuration["SqlConnectionString"]));

#else
            builder.Services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = Environment.GetEnvironmentVariable("RedisConnectionString");
                options.InstanceName = "InnovationGame_";
            });

            builder.Services.AddDbContext<GameContext>(options =>
                options.UseSqlServer(Environment.GetEnvironmentVariable("SqlConnectionString")));
#endif

        }
    }
}
