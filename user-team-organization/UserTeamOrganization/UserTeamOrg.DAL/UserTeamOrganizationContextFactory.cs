using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace UserTeamOrg.DAL
{
    public class UserTeamOrganizationContextFactory : IDesignTimeDbContextFactory<UserTeamOrganizationContext>
    { 
        public UserTeamOrganizationContext CreateDbContext(string[] args)
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("local.settings.json", true, true)
                .Build();

            string connection = configuration["SqlConnectionString"];
            var optionsBuilder = new DbContextOptionsBuilder<UserTeamOrganizationContext>();

            optionsBuilder.UseSqlServer(connection);

            return new UserTeamOrganizationContext(optionsBuilder.Options);
        }
    }
}

