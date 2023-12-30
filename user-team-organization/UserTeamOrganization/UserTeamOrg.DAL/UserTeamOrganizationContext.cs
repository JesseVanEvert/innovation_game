using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserTeamOrg.Model;
using UserTeamOrg.Model.Entity;
using Microsoft.EntityFrameworkCore;


namespace UserTeamOrg.DAL
{
    public class UserTeamOrganizationContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Admin> Admins { get; set; }
        public DbSet<UserTeam> UserTeams { get; set; }
        public DbSet<Person> Persons { get; set; }
        public DbSet<Team> Teams { get; set; }
        public DbSet<Organization> Organizations { get; set; }

        public UserTeamOrganizationContext(DbContextOptions<UserTeamOrganizationContext> options) : base(options) { }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<UserTeam>()
                .HasKey(gu => new { gu.UserId, gu.TeamId });
            modelBuilder.Entity<Person>()
                .HasOne(u => u.User)
                .WithOne(p => p.Person)
                .HasForeignKey<User>(k => k.PersonId);
            modelBuilder.Entity<Person>()
                .HasOne(a => a.Admin)
                .WithOne(p => p.Person)
                .HasForeignKey<Admin>(k => k.PersonId);
        }
    }
}

