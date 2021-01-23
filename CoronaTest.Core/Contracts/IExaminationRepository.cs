using CoronaTest.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CoronaTest.Core.Contracts
{
    public interface IExaminationRepository
    {
        Task AddAsync(Examination examination);
        Task AddRangeAsync(Examination[] examinations);
    }
}
