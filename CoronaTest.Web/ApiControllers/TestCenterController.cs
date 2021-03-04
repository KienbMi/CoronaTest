using CoronaTest.Core.Contracts;
using CoronaTest.Core.Models;
using CoronaTest.Web.DataTransferObjects;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CoronaTest.Web.ApiControllers
{
    /// <summary>
    /// API-Controller für die Abfrage von den TestCentern
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Authorize()]

    public class TestCenterController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;

        /// <summary>
        /// Constructor mit DI
        /// </summary>
        /// <param name="unitOfWork"></param>
        public TestCenterController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        /// <summary>
        /// Liefert alle vorhandenen TestCenter
        /// </summary>
        /// <returns></returns>
        /// <response code="200">Die Abfrage war erfolgreich</response>
        [HttpGet]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<TestCenterDto[]>> GetTestCenter()
        {
            TestCenterDto[] testCenters = (await _unitOfWork.TestCenters.GetAllAsync())
                .Select(_ => new TestCenterDto(_))
                .ToArray();

            return Ok(testCenters);
        }

        /// <summary>
        /// Hinzufügen eines neuen TestCenters
        /// </summary>
        /// <param name="testCenterDto"></param>
        /// <returns></returns>
        [HttpPost()]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult> PostTestCenter(TestCenterDto testCenterDto)
        {
            if (testCenterDto == null)
            {
                return BadRequest();
            }

            if (testCenterDto.Id != 0)
            {
                return BadRequest("Id has to be 0");
            }

            var newTestCenter = new TestCenter
            {
                Name = testCenterDto.Name,
                City = testCenterDto.City,
                Postalcode = testCenterDto.Postalcode,
                Street = testCenterDto.Street,
                SlotCapacity = testCenterDto.SlotCapacity
            };

            try
            {
                await _unitOfWork.TestCenters.AddAsync(newTestCenter);
                await _unitOfWork.SaveChangesAsync();

                return CreatedAtAction(
                    nameof(GetTestCenter),
                    new { id = newTestCenter.Id },
                    new TestCenterDto(newTestCenter));
            }
            catch (ValidationException ex)
            {
                return BadRequest(ex);
            }
        }

        /// <summary>
        /// Liefert alle vorhandenen TestCenter eines bestimmten Bezirks
        /// </summary>
        /// <param name="postalcode"></param>
        /// <returns></returns>
        [HttpGet()]
        [Authorize(Roles = "Admin")]
        [Route("byPostalCode/{postalcode}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<TestCenterDto[]>> GetTestCenterByPostalcode(string postalcode)
        {
            if (string.IsNullOrEmpty(postalcode))
            {
                return BadRequest();
            }

            TestCenterDto[] testCenters = (await _unitOfWork.TestCenters.GetByPostalcodeAsync(postalcode))
                .Select(_ => new TestCenterDto(_))
                .ToArray();

            return Ok(testCenters);
        }

        /// <summary>
        /// Abfrage eines bestimmten TestCenters
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<TestCenterDto>> GetTestCenterById(int id)
        {
            var testCenterInDb = (await _unitOfWork.TestCenters.GetByIdAsync(id));

            if (testCenterInDb == null)
            {
                return NotFound();
            }

            return Ok(new TestCenterDto(testCenterInDb));
        }

        /// <summary>
        /// Änderung eines TestCenters
        /// </summary>
        /// <param name="id"></param>
        /// <param name="testCenterDto"></param>
        /// <returns></returns>
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult>UpdateTestCenter(int id, TestCenterDto testCenterDto)
        {
            TestCenter testCenterInDb = await _unitOfWork.TestCenters.GetByIdAsync(id);
            if (testCenterInDb == null)
            {
                return NotFound();
            }

            testCenterInDb.Name = testCenterDto.Name;
            testCenterInDb.City = testCenterDto.City;
            testCenterInDb.Postalcode = testCenterDto.Postalcode;
            testCenterInDb.Street = testCenterDto.Street;
            testCenterInDb.SlotCapacity = testCenterDto.SlotCapacity;

            try
            {
                await _unitOfWork.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

            return NoContent();
        }

        /// <summary>
        /// Löschen eines TestCenters
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult> DeleteTestCenter(int id)
        {
            TestCenter testCenterInDb = await _unitOfWork.TestCenters.GetByIdAsync(id);
            if (testCenterInDb == null)
            {
                return NotFound();
            }

            _unitOfWork.TestCenters.Delete(testCenterInDb);

            try
            {
                await _unitOfWork.SaveChangesAsync();
            }
            catch (ValidationException ex)
            {
                return BadRequest(ex.Message);
            }

            return NoContent();
        }

        /// <summary>
        /// Abfrage eines bestimmten TestCenters
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet()]
        [Route("{id}/Examinations")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<ExaminationDto[]>> GetExaminationsByTestCenterId(int id)
        {
            TestCenter testCenterInDb = await _unitOfWork.TestCenters.GetByIdAsync(id);
            if (testCenterInDb == null)
            {
                return NotFound();
            }

            ExaminationDto[] examinations = (await _unitOfWork.Examinations.GetByTestCenterIdAsync(testCenterInDb.Id))
                .Select(_ => new ExaminationDto(_))
                .ToArray();

            return Ok(examinations);
        }
    }
}
