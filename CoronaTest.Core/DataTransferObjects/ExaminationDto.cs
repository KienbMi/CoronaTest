using CoronaTest.Core.Models;
using System;

namespace CoronaTest.Core.DataTransferObjects
{
    public class ExaminationDto
    {
        public int Id { get; set; }
        public string ParticipantFullname { get; set; }
        public TestResult TestResult { get; set; }
        public DateTime ExaminationAt { get; set; }
        public ExaminationDto(Examination examination)
        {
            Id = examination.Id;
            ParticipantFullname = $"{examination.Participant.Firstname} {examination.Participant.Lastname}";
            TestResult = examination.Result;
            ExaminationAt = examination.ExaminationAt;
        }
    }
}
