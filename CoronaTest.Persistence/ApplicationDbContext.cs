using CoronaTest.Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Diagnostics;
using Utils;

namespace CoronaTest.Persistence
{
    public class ApplicationDbContext : DbContext
    {
        public DbSet<VerificationToken> VerificationTokens { get; set; }
        public DbSet<Campaign> Campaigns { get; set; }
        public DbSet<Examination> Examinations { get; set; }
        public DbSet<Participant> Participants { get; set; }
        public DbSet<TestCenter> TestCenters { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Environment.CurrentDirectory)
                .AddJsonFile("appsettings.json")
                .AddEnvironmentVariables()
                .Build();

            Debug.WriteLine(configuration);

            optionsBuilder.UseSqlServer(configuration["ConnectionStrings:DefaultConnection"]);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Role>().HasData(new Role { Id = 1, Name = "Admin" });
            modelBuilder.Entity<Role>().HasData(new Role { Id = 2, Name = "User" });
            modelBuilder.Entity<User>().HasData(new User
            {
                Id = 1,
                //RoleId = 1,
                UserRole = "Admin",
                Email = "admin@htl.at",
                Password = AuthUtils.GenerateHashedPassword("admin@htl.at")
            });
            modelBuilder.Entity<User>().HasData(new User
            {
                Id = 2,
                //RoleId = 2,
                UserRole = "User",
                Email = "user@htl.at",
                Password = AuthUtils.GenerateHashedPassword("user@htl.at")
            });
        }
    }
}
