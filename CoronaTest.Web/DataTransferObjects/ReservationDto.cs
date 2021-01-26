using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CoronaTest.Web.DataTransferObjects
{
    public class ReservationDto
    {
        [Range(1, int.MaxValue, ErrorMessage = "Bitte auswählen")]
        public int SelectedCampaignId { get; set; }
        public List<SelectListItem> Campaigns { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Bitte auswählen")]
        public int SelectedTestCenterId { get; set; }
        public List<SelectListItem> TestCenters { get; set; }

        public DateTime SelectedDay { get; set; }
        public List<SelectListItem> Days { get; set; }

        public DateTime SelectedSlot { get; set; }
        public List<SelectListItem> Slots { get; set; }
    }
}
