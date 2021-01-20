using CoronaTest.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CoronaTest.Core.Contracts
{
    public interface IVerificationTokenRepository
    {
        Task<VerificationToken> GetTokenByIdentifierAsync(Guid identifier);
        Task AddAsync(VerificationToken token);
    }
}
