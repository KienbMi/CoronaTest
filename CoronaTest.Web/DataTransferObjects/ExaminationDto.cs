using CoronaTest.Core;
using CoronaTest.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoronaTest.Web.DataTransferObjects
{
    public class ExaminationDto
    {
        public int Id { get; set; }
        public Campaign Campaign { get; set; }
        public Participant Participant { get; set; }
        public TestCenter TestCenter { get; set; }
        public TestResult Result { get; set; }
        public ExaminationStates State { get; set; }
        public DateTime ExaminationAt { get; set; }
        public string Identifier { get; set; }

        public ExaminationDto()
        {

        }

        public ExaminationDto(Examination examination)
        {
            Id = examination.Id;
            Campaign = examination.Campaign;
            Participant = examination.Participant;
            TestCenter = examination.TestCenter;
            Result = examination.Result;
            State = examination.State;
            ExaminationAt = examination.ExaminationAt;
            Identifier = examination.Identifier;
        }
    }
}
