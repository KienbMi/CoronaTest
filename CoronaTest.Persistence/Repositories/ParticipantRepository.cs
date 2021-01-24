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
    public class ParticipantRepository : IParticipantRepository
    {
        private readonly ApplicationDbContext _dbContext;

        public ParticipantRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task AddAsync(Participant participant)
            => await _dbContext
                .Participants
                .AddAsync(participant);

        public async Task AddRangeAsync(Participant[] participants)
            => await _dbContext
                .Participants
                .AddRangeAsync(participants);

        public async Task<Participant> GetByIdAsync(int id)
            => await _dbContext
                .Participants
                .Where(p => p.Id == id)
                .SingleOrDefaultAsync();
    }
}
