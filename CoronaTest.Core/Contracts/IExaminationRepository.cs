﻿using CoronaTest.Core.DataTransferObjects;
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
        Task<SlotDto[]> GetSlotsAsync(int campaignId, int testCenterId);
        Task<Examination[]> GetByCampaignTestCenter(Campaign campaign, TestCenter testCenter);
        Task<Examination[]> GetByParticipantIdAsync(int participantId);
        Task<Examination> GetByIdAsync(int id);
        void Remove(Examination examination);
    }
}
