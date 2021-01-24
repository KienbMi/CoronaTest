using CoronaTest.Core.Contracts;
using CoronaTest.Core.Models;
using CoronaTest.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoronaTest.Persistence
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _dbContext;
        private bool _disposed;

        public ICampaignRepository Campaigns { get; }
        public ITestCenterRepository TestCenters { get; }
        public IParticipantRepository Participants { get; }
        public IVerificationTokenRepository VerificationTokens { get; }
        public IExaminationRepository Examinations { get; }

        public UnitOfWork() : this(new ApplicationDbContext())
        {
        }

        public UnitOfWork(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;

            Campaigns = new CampaignRepository(_dbContext);
            TestCenters = new TestCenterRepository(_dbContext);
            Participants = new ParticipantRepository(_dbContext);
            VerificationTokens = new VerificationTokenRepository(_dbContext);
            Examinations = new ExaminationRepository(_dbContext);
        }


        public async Task<int> SaveChangesAsync()
        {
            var entities = _dbContext.ChangeTracker.Entries()
                .Where(entity => entity.State == EntityState.Added
                                 || entity.State == EntityState.Modified)
                .Select(e => e.Entity);
            foreach (var entity in entities)
            {
                await ValidateEntity(entity);
            }
            return await _dbContext.SaveChangesAsync();
        }

        /// <summary>
        /// Validierungen auf DbContext-Ebene
        /// </summary>
        /// <param name="entity"></param>
        private async Task ValidateEntity(object entity)
        {
            if (entity is Participant participant)
            {
                if (await _dbContext.Participants.AnyAsync(p => p.Id != participant.Id && p.SocialSecurityNumber == participant.SocialSecurityNumber))
                {
                    throw new ValidationException($"Eine Person mit der Sozialversicherungsnummer {participant.SocialSecurityNumber} ist bereits registriert.");
                }
            }
        }

        public async Task DeleteDatabaseAsync() => await _dbContext.Database.EnsureDeletedAsync();
        public async Task MigrateDatabaseAsync() => await _dbContext.Database.MigrateAsync();
        public async Task CreateDatabaseAsync() => await _dbContext.Database.EnsureCreatedAsync();

        public async ValueTask DisposeAsync()
        {
            await DisposeAsync(true);
            GC.SuppressFinalize(this);
        }

        protected virtual async ValueTask DisposeAsync(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    await _dbContext.DisposeAsync();
                }
            }
            _disposed = true;
        }
    }
}
