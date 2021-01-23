﻿using CoronaTest.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CoronaTest.Core.Contracts
{
    public interface ICampaignRepository
    {
        Task AddAsync(Campaign campaign);
        Task AddRangeAsync(Campaign[] campaigns);
        Task<int> GetCountAsync();
    }
}