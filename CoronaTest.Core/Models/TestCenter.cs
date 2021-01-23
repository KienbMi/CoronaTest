using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace CoronaTest.Core.Models
{
    public class TestCenter : EntityBase
    {
        public string Name { get; set; }
        public string City { get; set; }
        public string Postalcode { get; set; }
        public string Street { get; set; }
        public int SlotCapacity { get; set; }
        
        public ICollection<Campaign> AvailableInCampaigns { get; set; }
    }
}
