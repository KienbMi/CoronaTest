using System;
using System.Collections.Generic;
using System.Text;

namespace CoronaTest.Core.Models
{
    public class Examination : EntityBase
    {
        public Campaign Campaign { get; set; }
        public Participant Participant { get; set; }
        public TestCenter TestCenter { get; set; }
        public TestResult Result { get; set; }
        public ExaminationStates State { get; set; }
        public DateTime ExaminationAt { get; set; }
        public string Identifier { get; set; }

        public static Examination CreateNew()
        {
            return new Examination();
        }
    }
}
