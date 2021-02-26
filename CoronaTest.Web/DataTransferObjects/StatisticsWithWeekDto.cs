using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoronaTest.Web.DataTransferObjects
{
    public class StatisticsWithWeekDto : StatisticsDto
    {
        public int Year { get; set; }
        public int WeekNumber { get; set; }
    }
}
