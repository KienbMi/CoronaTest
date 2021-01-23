using CoronaTest.Core.Contracts;
using CoronaTest.Core.Models;
using System;
using System.Collections.Generic;
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
    }
}
