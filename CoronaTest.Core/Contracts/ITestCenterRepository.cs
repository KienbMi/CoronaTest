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
    }
}
