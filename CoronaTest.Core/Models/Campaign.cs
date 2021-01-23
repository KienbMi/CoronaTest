using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace CoronaTest.Core.Models
{
    public class Campaign : EntityBase
    {
        public string Name { get; set; }
        public DateTime From { get; set; }
        public DateTime To { get; set; }
        
        public ICollection<TestCenter> AvailableTestCenters { get; set; }
    }
}
