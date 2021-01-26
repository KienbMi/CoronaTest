using CoronaTest.Core.DataTransferObjects;
using CoronaTest.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CoronaTest.Core.Contracts
{
    public interface ITestCenterRepository
    {
        Task AddAsync(TestCenter testCenter);
        Task AddRangeAsync(TestCenter[] testCenters);
        Task<int> GetCountAsync();
        Task<TestCenter[]> GetByCampaignIdAsync(int campaignId);
        Task<TestCenter> GetByIdAsync(int id);
        Task<IEnumerable<SlotDto>> GetAllSlotsByCampaignIdAsync(int campaignId, int testCenterId);
    }
}
