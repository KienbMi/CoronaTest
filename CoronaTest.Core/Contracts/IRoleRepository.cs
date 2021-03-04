using CoronaTest.Core.DataTransferObjects;
using CoronaTest.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CoronaTest.Core.Contracts
{
    public interface IRoleRepository
    {
        Task AddAsync(Role role);
    }
}
