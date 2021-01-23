using CoronaTest.Core.Contracts;
using CoronaTest.Core.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CoronaTest.Persistence.Repositories
{
    public class TestCenterRepository : ITestCenterRepository
    {
        private readonly ApplicationDbContext _dbContext;

        public TestCenterRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task AddAsync(TestCenter testCenter)
            => await _dbContext
                .TestCenters
                .AddAsync(testCenter);

        public async Task AddRangeAsync(TestCenter[] testCenters)
            => await _dbContext
                .TestCenters
                .AddRangeAsync(testCenters);

        public async Task<int> GetCountAsync()
            => await _dbContext
                .TestCenters
                .CountAsync();
    }
}
