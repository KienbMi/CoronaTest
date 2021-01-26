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
        private static string _cbDefaultText = "Bitte auswählen";
        private static int _cbDefaultValue = 0;
        private static DateTime _cbDefaultDate = default(DateTime);

        public int SelectedCampaignId { get; set; }
        public List<SelectListItem> Campaigns { get; set; } = new List<SelectListItem>
                                { new SelectListItem(_cbDefaultText, _cbDefaultValue.ToString())};

        public int SelectedTestCenterId { get; set; }
        public List<SelectListItem> TestCenters { get; set; } = new List<SelectListItem>
                                { new SelectListItem(_cbDefaultText, _cbDefaultValue.ToString())};

        public DateTime SelectedDay { get; set; }
        public List<SelectListItem> Days { get; set; } = new List<SelectListItem>
                                { new SelectListItem(_cbDefaultText, _cbDefaultDate.ToString())};

        public DateTime SelectedSlot { get; set; }
        public List<SelectListItem> Slots { get; set; } = new List<SelectListItem>
                                { new SelectListItem(_cbDefaultText, _cbDefaultDate.ToString())};
    }
}
