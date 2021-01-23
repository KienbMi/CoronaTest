using CoronaTest.Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Diagnostics;

namespace CoronaTest.Persistence
{
    public class ApplicationDbContext : DbContext
    {
        public DbSet<VerificationToken> VerificationTokens { get; set; }
        public DbSet<Campaign> Campaigns { get; set; }
        public DbSet<Examination> Examinations { get; set; }
        public DbSet<Participant> Participants { get; set; }
        public DbSet<TestCenter> TestCenters { get; set; }

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

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<Campaign>()
                        .HasMany(_ => _.AvailableTestCenters);

            builder.Entity<TestCenter>()
                        .HasMany(_ => _.AvailableInCampaigns);
        }
    }
}
