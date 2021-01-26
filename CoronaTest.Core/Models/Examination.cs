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

        public string GetReservationText()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append($"CoronaTest - Identifier: {Identifier} ");
            sb.Append($"für Ihren Termin am {ExaminationAt.ToShortDateString()} ");
            sb.Append($"um {ExaminationAt.ToShortTimeString()} ");
            sb.Append($"im TestCenter: {TestCenter?.Name}!");

            return sb.ToString();
        }

        public string GetCancelText()
        { 
            StringBuilder sb = new StringBuilder();
            sb.Append($"Storno - Identifier: {Identifier} ");
            sb.Append($"Ihr Termin am {ExaminationAt.ToShortDateString()} ");
            sb.Append($"um {ExaminationAt.ToShortTimeString()} ");
            sb.Append($"im TestCenter: {TestCenter?.Name} wurde storniert!");
                 
            return sb.ToString();
        }
    }
}
