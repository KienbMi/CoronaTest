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
    public class CampaignRepository : ICampaignRepository
    {
        private readonly ApplicationDbContext _dbContext;

        public CampaignRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task AddAsync(Campaign campaign)
            => await _dbContext
                .Campaigns
                .AddAsync(campaign);

        public async Task AddRangeAsync(Campaign[] campaigns)
            => await _dbContext
                .Campaigns
                .AddRangeAsync(campaigns);

        public async Task<Campaign[]> GetAllAsync()
            => await _dbContext
                .Campaigns
                .OrderBy(c => c.Name)
                .ToArrayAsync();

        public async Task<Campaign> GetByIdAsync(int id)
            => await _dbContext
                .Campaigns
                .SingleOrDefaultAsync(c => c.Id == id);

        public async Task<int> GetCountAsync()
            => await _dbContext
                .Campaigns
                .CountAsync();
    }
}
