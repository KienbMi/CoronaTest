using CoronaTest.Core;
using CoronaTest.Core.Contracts;
using CoronaTest.Web.DataTransferObjects;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace CoronaTest.Web.ApiControllers
{
    /// <summary>
    /// API-Controller für die Abfrage von Statistiken
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class StatisticsController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;

        /// <summary>
        /// Constructor mit DI
        /// </summary>
        /// <param name="unitOfWork"></param>
        public StatisticsController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        /// <summary>
        /// Liefert die Teststatistik im Zeitraum
        /// </summary>
        /// <response code="200">Die Abfrage war erfolgreich.</response>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<StatisticsDto>> GetStatisticsInPeriode(DateTime from, DateTime to)
        {
            var examinations = await _unitOfWork.Examinations.GetExaminationsWithFilterAsync(null, from, to);

            StatisticsDto statistic = new StatisticsDto();
            
            if (examinations != null)
            {
                statistic.CountOfExaminations = examinations.Count();
                statistic.CountOfUnknownResult = examinations.Count(_ => _.Result == TestResult.Unknown);
                statistic.CountOfPositiveResult = examinations.Count(_ => _.Result == TestResult.Positive);
                statistic.CountOfNegativeResult = examinations.Count(_ => _.Result == TestResult.Negative);
            }

            return Ok(statistic);
        }

        /// <summary>
        /// Liefert die Teststatistik in der Gemeinde und im Zeitraum
        /// </summary>
        /// <response code="200">Die Abfrage war erfolgreich.</response>
        [HttpGet]
        [Route("byPostalCode")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<StatisticsDto>> GetStatisticsInAreaAndPeriode(string postalCode, DateTime from, DateTime to)
        {
            var examinations = await _unitOfWork.Examinations.GetExaminationsWithFilterAsync(postalCode, from, to);

            StatisticsDto statistic = new StatisticsDto();

            if (examinations != null)
            {
                statistic.CountOfExaminations = examinations.Count();
                statistic.CountOfUnknownResult = examinations.Count(_ => _.Result == TestResult.Unknown);
                statistic.CountOfPositiveResult = examinations.Count(_ => _.Result == TestResult.Positive);
                statistic.CountOfNegativeResult = examinations.Count(_ => _.Result == TestResult.Negative);
            }

            return Ok(statistic);
        }

        /// <summary>
        /// Liefert die Teststatistik je Kalenderwochen im Zeitraum
        /// </summary>
        /// <response code="200">Die Abfrage war erfolgreich.</response>
        [HttpGet]
        [Route("perCalendarWeek")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<StatisticsWithWeekDto[]>> GetStatisticsWithWeekNumberInPeriode(DateTime from, DateTime to)
        {
            var examinations = await _unitOfWork.Examinations.GetExaminationsWithFilterAsync(null, from, to);

            StatisticsWithWeekDto[] statistic = new StatisticsWithWeekDto[0];
            Calendar calendar = CultureInfo.InvariantCulture.Calendar;

            statistic = examinations.GroupBy(_ => $"{_.ExaminationAt.Year}_{calendar.GetWeekOfYear(_.ExaminationAt, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday):d2}")
                .Select(_ => new StatisticsWithWeekDto {
                    Year = int.Parse(_.Key.Split('_')[0]),
                    WeekNumber = int.Parse(_.Key.Split('_')[1]),
                    CountOfExaminations = _.Count(),
                    CountOfUnknownResult = _.Count(_ => _.Result == TestResult.Unknown),
                    CountOfPositiveResult = _.Count(_ => _.Result == TestResult.Positive),
                    CountOfNegativeResult = _.Count(_ => _.Result == TestResult.Negative)
                })
                .OrderBy(_ => _.Year)
                .ThenBy(_ => _.WeekNumber)
                .ToArray();

            return Ok(statistic);
        }
    }
}
