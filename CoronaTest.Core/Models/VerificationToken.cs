using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace CoronaTest.Core.Models
{
    public class VerificationToken : EntityBase
    {
        public int Token { get; set; }
        public Participant Participant { get; set; }
        public Guid Identifier { get; set; }
        public DateTime IssuedAt { get; set; }
        
        [NotMapped]
        public DateTime ValidUntil => IssuedAt.AddMinutes(15);

        public bool IsInvalidated { get; set; }

        public static VerificationToken NewToken(Participant participant)
        {
            return new VerificationToken()
            {
                Token = new Random().Next(100000, 1000000),
                Participant = participant,
                Identifier = Guid.NewGuid(),
                IssuedAt = DateTime.Now,
                IsInvalidated = false
            };
        }
    }
}
