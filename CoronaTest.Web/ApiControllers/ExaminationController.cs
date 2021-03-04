using CoronaTest.Core.Contracts;
using CoronaTest.Web.DataTransferObjects;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoronaTest.Web.ApiControllers
{
    /// <summary>
    /// API-Controller für die Abfrage von Kampagnen
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Authorize()]
    public class ExaminationController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;

        /// <summary>
        /// Constructor mit DI
        /// </summary>
        /// <param name="unitOfWork"></param>
        public ExaminationController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        /// <summary>
        /// Liefert alle Untersuchungen im Zeitraum
        /// </summary>
        /// <response code="200">Die Abfrage war erfolgreich.</response>
        [HttpGet]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<ExaminationDto[]>> GetExaminationsInPeriode(DateTime from, DateTime to)
        {
            ExaminationDto[] examinations = (await _unitOfWork.Examinations.GetExaminationsWithFilterAsync(null, from, to))
                .Select(_ => new ExaminationDto(_))
                .ToArray();

            return Ok(examinations);
        }

        /// <summary>
        /// Liefert alle Untersuchungen in der Gemeinde und im Zeitraum
        /// </summary>
        /// <response code="200">Die Abfrage war erfolgreich.</response>
        [HttpGet]
        [Authorize(Roles = "Admin")]
        [Route("byPostalCode")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<ExaminationDto[]>> GetExaminationsInAreaAndPeriode(string postalCode, DateTime from, DateTime to)
        {
            ExaminationDto[] examinations = (await _unitOfWork.Examinations.GetExaminationsWithFilterAsync(postalCode, from, to))
                .Select(_ => new ExaminationDto(_))
                .ToArray();

            return Ok(examinations);
        }
    }
}
