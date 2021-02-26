using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoronaTest.Web.DataTransferObjects
{
    public class StatisticsDto
    {
        public int CountOfExaminations { get; set; }
        public int CountOfUnknownResult { get; set; }
        public int CountOfPositiveResult { get; set; }
        public int CountOfNegativeResult { get; set; }
    }
}
