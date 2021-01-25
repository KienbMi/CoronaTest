using CoronaTest.Core.Contracts;
using CoronaTest.Core.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
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

        public async Task<TestCenter[]> GetByCampaignIdAsync(int campaignId)
        {
            var campaign = await _dbContext
                .Campaigns
                .Include(_ => _.AvailableTestCenters)
                .SingleAsync(c => c.Id == campaignId);

            return campaign.AvailableTestCenters
                .OrderBy(_ => _.Name)
                .ToArray();
        }

        public async Task<TestCenter> GetByIdAsync(int id)
            => await _dbContext
                .TestCenters
                .SingleOrDefaultAsync(_ => _.Id == id);

        public async Task<int> GetCountAsync()
            => await _dbContext
                .TestCenters
                .CountAsync();
    }
}
