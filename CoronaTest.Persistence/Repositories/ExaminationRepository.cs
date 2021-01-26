using CoronaTest.Core.Contracts;
using CoronaTest.Core.DataTransferObjects;
using CoronaTest.Core.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoronaTest.Persistence.Repositories
{
    public class ExaminationRepository: IExaminationRepository
    {
        private readonly ApplicationDbContext _dbContext;

        public ExaminationRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task AddAsync(Examination examination)
            => await _dbContext
                .Examinations
                .AddAsync(examination);

        public async Task AddRangeAsync(Examination[] examinations)
            => await _dbContext
                .Examinations
                .AddRangeAsync(examinations);

        public async Task<Examination[]> GetByCampaignTestCenter(Campaign campaign, TestCenter testCenter)
            => await _dbContext
                .Examinations
                .Where(_ => _.Campaign == campaign && _.TestCenter == testCenter)
                .OrderBy(_ => _.ExaminationAt)
                .ToArrayAsync();

        public async Task<Examination> GetByIdAsync(int id)
            => await _dbContext
                .Examinations
                .Include(_ => _.TestCenter)
                .Include(_ => _.Campaign)
                .Include(_ => _.Participant)
                .SingleOrDefaultAsync(_ => _.Id == id);
                
        public async Task<Examination[]> GetByParticipantIdAsync(int participantId)
            => await _dbContext
                .Examinations
                .Include(_ => _.Campaign)
                .Include(_ => _.TestCenter)
                .Where(_ => _.Participant.Id == participantId)
                .OrderBy(_ => _.ExaminationAt)
                .ToArrayAsync();

        public void Remove(Examination examination)
            => _dbContext
                .Examinations
                .Remove(examination);
    }
}
