using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using CoronaTest.Core.Contracts;
using CoronaTest.Core.Models;
using CoronaTest.Web.DataTransferObjects;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CoronaTest.Web.ApiControllers
{
    /// <summary>
    /// API-Controller für die Abfrage von Kampagnen
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Authorize()]
    public class CampaignController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;

        /// <summary>
        /// Constructor mit DI
        /// </summary>
        /// <param name="unitOfWork"></param>
        public CampaignController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        /// <summary>
        /// Liefert alle vorhandenen Kampagnen
        /// </summary>
        /// <response code="200">Die Abfrage war erfolgreich.</response>
        [HttpGet]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<CampaignDto[]>> GetCampaigns()
        {
            CampaignDto[] campaigns = (await _unitOfWork.Campaigns.GetAllAsync())
                .Select(_ => new CampaignDto(_))
                .ToArray();
            return Ok(campaigns);
        }

        /// <summary>
        /// Hinzufügen einer neuen Kampagne
        /// </summary>
        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult> PostCampaign([FromBody] CampaignDto campaignDto)
        {
            if (campaignDto == null)
            {
                return BadRequest();
            }

            if (campaignDto.Id != 0)
            {
                return BadRequest("Id has to be 0");
            }

            try
            {
                var newCampaign = new Campaign
                {
                    Name = campaignDto.Name,
                    From = campaignDto.From,
                    To = campaignDto.To
                };

                await _unitOfWork.Campaigns.AddAsync(newCampaign);
                await _unitOfWork.SaveChangesAsync();
                return CreatedAtAction(
                    nameof(GetCampaignById),
                    new { id = newCampaign.Id },
                    new CampaignDto(newCampaign));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Abfrage einer bestimmten Kampagne
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> GetCampaignById(int id)
        {
            var campaignInDb = await _unitOfWork.Campaigns.GetByIdAsync(id);

            if (campaignInDb == null)
            {
                return NotFound();
            }

            return Ok(new CampaignDto(campaignInDb));
        }

        /// <summary>
        /// Änderung einer Kampagne
        /// </summary>
        /// <param name="id"></param>
        /// <param name="campaignName"></param>
        /// <returns></returns>
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult> UpdateCampaign(int id, string campaignName)
        {
            Campaign campaignInDb = await _unitOfWork.Campaigns.GetByIdAsync(id);
            if (campaignInDb == null)
            {
                return NotFound();
            }

            campaignInDb.Name = campaignName;

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
        /// Löschen einer Kampagne
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
        public async Task<ActionResult> DeleteCampaign(int id)
        {
            Campaign campaignInDb = await _unitOfWork.Campaigns.GetByIdAsync(id);
            if (campaignInDb == null)
            {
                return NotFound();
            }

            _unitOfWork.Campaigns.Delete(campaignInDb);

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
        /// Abfrage der Untersuchungen einer bestimmten Kampagne
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
        public async Task<ActionResult<ExaminationDto[]>> GetExaminationsByCampaignId(int id)
        {
            var campainInDb = await _unitOfWork.Campaigns.GetByIdAsync(id);
            if (campainInDb == null)
            {
                return NotFound();
            }

            ExaminationDto[] examinations = (await _unitOfWork.Examinations.GetByCampaignIdAsync(campainInDb.Id))
                .Select(_ => new ExaminationDto(_))
                .ToArray();

            return Ok(examinations);
        }

        /// <summary>
        /// Abfrage der TestCenter einer bestimmten Kampagne
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet()]
        [Route("{id}/TestCenters")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<ExaminationDto[]>> GetTestCentersByCampaignId(int id)
        {
            var campainInDb = await _unitOfWork.Campaigns.GetByIdAsync(id);
            if (campainInDb == null)
            {
                return NotFound();
            }

            TestCenterDto[] testCenters = (await _unitOfWork.TestCenters.GetByCampaignIdAsync(campainInDb.Id))
                .Select(_ => new TestCenterDto(_))
                .ToArray();

            return Ok(testCenters);
        }

        /// <summary>
        /// Schaltet ein TestCenter für eine Kampagne frei
        /// </summary>
        /// <param name="id"></param>
        /// <param name="testCenterIdToAdd"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("{id}/TestCenters/{testCenterIdToAdd}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult> PostTestCenterToCampaign(int id, int testCenterIdToAdd)
        {
            var campaignInDb = await _unitOfWork.Campaigns.GetByIdAsync(id);
            if (campaignInDb == null)
            {
                return NotFound("Campaign id does not exist");
            }

            var testCenterInDb = await _unitOfWork.TestCenters.GetByIdAsync(testCenterIdToAdd);
            if (testCenterInDb == null)
            {
                return NotFound("TestCenter id does not exist");
            }

            if (campaignInDb.AvailableTestCenters.Contains(testCenterInDb))
            {
                return BadRequest("TestCenter is already added");
            }

            campaignInDb.AvailableTestCenters.Add(testCenterInDb);

            try
            {
                await _unitOfWork.SaveChangesAsync();
            }
            catch (ValidationException ex)
            {
                return BadRequest(ex.Message);
            }

            return CreatedAtAction(
                    nameof(GetCampaignById),
                    new { id = campaignInDb.Id },
                    new CampaignDto(campaignInDb));
        }

        [HttpDelete]
        [Route("{id}/TestCenters/{testCenterIdToRemove}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult>DeleteTestCenterFromCampaign(int id, int testCenterIdToRemove)
        {
            var campaignInDb = await _unitOfWork.Campaigns.GetByIdAsync(id);
            if (campaignInDb == null)
            {
                return NotFound("Campaign id does not exist");
            }

            var testCenterInDb = await _unitOfWork.TestCenters.GetByIdAsync(testCenterIdToRemove);
            if (testCenterInDb == null)
            {
                return NotFound("TestCenter id does not exist");
            }

            if (!campaignInDb.AvailableTestCenters.Contains(testCenterInDb))
            {
                return BadRequest("TestCenter not in list");
            }

            campaignInDb.AvailableTestCenters.Remove(testCenterInDb);

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
    }
}
