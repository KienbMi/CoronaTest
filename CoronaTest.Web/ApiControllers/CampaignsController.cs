using System.Linq;
using System.Threading.Tasks;
using CoronaTest.Core.Contracts;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace SmartSchool.Web.ApiControllers
{
    /// <summary>
    /// API-Controller für die Abfrage von Mitgliedern
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class CampaignsController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;

        /// <summary>
        /// Constructor mit DI
        /// </summary>
        /// <param name="unitOfWork"></param>
        public CampaignsController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        /// <summary>
        /// Liefert alle Namen der Sensoren
        /// </summary>
        /// <response code="200">Die Abfrage war erfolgreich.</response>
        // GET: api/Campaigns
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<string>> GetCampaignNames()
        {
            //Sensor[] sensors = await _unitOfWork.SensorRepository.GetAllAsync();
            return "Das ist der erste Test";
        }
    }
}
