using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CoronaTest.Core.Contracts;
using CoronaTest.Core.DataTransferObjects;
using CoronaTest.Core.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace CoronaTest.Web.Pages.User
{
    public class ReservationModel : PageModel
    {
        private readonly IUnitOfWork _unitOfWork;
        private string _cbDefaultText = "Bitte auswählen";
        private int _cbDefaultValue = -1;

        [BindProperty]
        public int SelectedCampaignId { get; set; }
        public List<SelectListItem> Campaigns { get; set; }
        
        [BindProperty]
        public int SelectedTestCenterId { get; set; }
        public List<SelectListItem> TestCenters { get; set; }

        [BindProperty]
        public DateTime SelectedDay { get; set; }
        public List<SelectListItem> Days { get; set; }

        [BindProperty]
        public DateTime SelectedSlot { get; set; }
        public List<SelectListItem> Slots { get; set; }


        public ReservationModel(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task OnGetAsync()
        {
            Campaigns = new List<SelectListItem>{
                new SelectListItem(_cbDefaultText, _cbDefaultValue.ToString())
                };

            var campaigns = await _unitOfWork.Campaigns.GetAllAsync();
            Campaigns.AddRange(campaigns
                .Select(campaign => new SelectListItem
                                (campaign.Name, campaign.Id.ToString())));      
        }



        public async Task OnPostAsync()
        {
            List<SlotDto> availableSlots = new List<SlotDto>();

            Campaigns = new List<SelectListItem>{
                new SelectListItem(_cbDefaultText, _cbDefaultValue.ToString())};

            var campaigns = await _unitOfWork.Campaigns.GetAllAsync();
            Campaigns.AddRange(campaigns
                .Select(campaign => new SelectListItem
                                (campaign.Name, campaign.Id.ToString())));

            if (SelectedCampaignId > 0)
            {
                TestCenters = new List<SelectListItem>{
                    new SelectListItem(_cbDefaultText, _cbDefaultValue.ToString())};

                var testCenters = await _unitOfWork.TestCenters.GetByCampaignIdAsync(SelectedCampaignId);
                TestCenters.AddRange(testCenters
                    .Select(testCenter => new SelectListItem
                                    (testCenter.Name, testCenter.Id.ToString())));
            }

            if (SelectedTestCenterId > 0)
            {
                Days = new List<SelectListItem>{
                    new SelectListItem(_cbDefaultText, _cbDefaultValue.ToString())};

                //SlotDto[] slots = await _unitOfWork.Examinations.GetSlotsAsync(SelectedCampaignId, SelectedTestCenterId);

                //var campaign = await _unitOfWork.Campaigns.GetByIdAsync(SelectedCampaignId);

                DateTime runDate = DateTime.Parse("03.02.2021");
                DateTime endDate = DateTime.Parse("08.02.2021");

                while (runDate.Date < endDate.Date)
                {
                    availableSlots.Add(new SlotDto 
                    {
                        Time = runDate,
                        SlotsAvailable = 7}
                    );
                    runDate = runDate.AddDays(1);
                }

                Days.AddRange(availableSlots
                    .Select(slot => new SelectListItem
                                    (slot.Time.ToShortDateString(), slot.Time.ToShortDateString())));

            }

            if (SelectedDay != null)
            {
                Slots = new List<SelectListItem>{
                    new SelectListItem(_cbDefaultText, _cbDefaultValue.ToString())};

                DateTime runDate = SelectedDay.AddHours(8);
                availableSlots.Clear();

                while (runDate <= SelectedDay.AddHours(16).AddMinutes(45))
                {
                    availableSlots.Add(new SlotDto
                    {
                        Time = runDate,
                        SlotsAvailable = 7
                    });
                    runDate = runDate.AddMinutes(15);
                }



                availableSlots.Where(_ => _.Time.Date == SelectedDay.Date);

                Slots.AddRange(availableSlots
                    .Select(slot => new SelectListItem
                    ($"{slot.Time.ToShortTimeString()} | Available: {slot.SlotsAvailable}", slot.Time.ToString())));
            }
        }
    }
}
