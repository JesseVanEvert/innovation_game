using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace CardDeck.DAL
{
    public class CardContextFactory : IDesignTimeDbContextFactory<CardContext>
    {
        public CardContext CreateDbContext(string[] args)
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("local.settings.json", true, true)
                .Build();

            string connection = configuration["SqlConnectionString"];
            var optionsBuilder = new DbContextOptionsBuilder<CardContext>();

            optionsBuilder.UseSqlServer(connection);

            return new CardContext(optionsBuilder.Options);
        }
    }
}
