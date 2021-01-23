using System;
using System.Collections.Generic;
using System.Text;

namespace CoronaTest.Core.Models
{
    public class Participant : EntityBase
    {
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public string Gender { get; set; }
        public string SocialSecurityNumber { get; set; }
        public DateTime Birthdate { get; set; }
        public string Mobilephone { get; set; }
        public string Postalcode { get; set; }
        public string City { get; set; }
        public string Street { get; set; }
        public string HouseNumber { get; set; }
        public string Stair { get; set; }
        public string Door { get; set; }
        public List<VerificationToken> Verifications { get; set; }
    }
}
