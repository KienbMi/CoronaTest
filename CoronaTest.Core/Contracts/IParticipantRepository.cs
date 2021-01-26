using CoronaTest.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CoronaTest.Core.Contracts
{
    public interface IParticipantRepository
    {
        Task AddAsync(Participant participant);
        Task AddRangeAsync(Participant[] participants);
        Task<Participant> GetByIdAsync(int id);
        Task<Participant> GetBySocialSecurityNumberAsync(string socialSecurityNumber);
    }
}
