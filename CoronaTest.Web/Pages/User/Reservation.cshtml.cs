using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using CoronaTest.Core;
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
        private string _cbDefaultText = "Bitte ausw�hlen";
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
            IEnumerable<SlotDto> allSlots = new List<SlotDto>();
            IEnumerable<SlotDto> availableSlots = new List<SlotDto>();

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

                allSlots = await GetAllSlotsAsync(SelectedCampaignId, SelectedTestCenterId);

                availableSlots = allSlots
                    .Where(_ => _.SlotsAvailable > 0)
                    .ToList();

                Days.AddRange(availableSlots
                    .GroupBy(_ => _.Time.Date)
                    .Select(group => new SelectListItem
                                    (group.Key.Date.ToShortDateString(), group.Key.Date.ToShortDateString())));
            }

            if (SelectedDay != default(DateTime))
            {
                Slots = new List<SelectListItem>{
                    new SelectListItem(_cbDefaultText, _cbDefaultValue.ToString())};

                Slots.AddRange(availableSlots
                    .Where(_ => _.Time.Date == SelectedDay)
                    .Select(slot => new SelectListItem
                    ($"{slot.Time.ToShortTimeString()} | Verf�gbar: {slot.SlotsAvailable}", slot.Time.ToString())));
            }
        }

        public async Task<IActionResult> OnPostSaveAsync()
        {
            if(!ModelState.IsValid)
            {
                return Page();
            }

            var examination = new Examination
            {
                Campaign = await _unitOfWork.Campaigns.GetByIdAsync(SelectedCampaignId),
                Participant = await _unitOfWork.Participants.GetByIdAsync(1), 
                TestCenter = await _unitOfWork.TestCenters.GetByIdAsync(SelectedTestCenterId),
                Result = TestResult.Unknown,
                State = ExaminationStates.New,
                ExaminationAt = SelectedDay.AddMinutes(SelectedSlot.TimeOfDay.TotalMinutes),
                Identifier = "0"
            };

            await _unitOfWork.Examinations.AddAsync(examination);

            try
            {
                await _unitOfWork.SaveChangesAsync();
            }
            catch (ValidationException ex)
            {
                ModelState.AddModelError("", $"{ex.Message}");
                return Page();
            }

            //_smsService.SendSms(Mobilenumber, $"CoronaTest - Token: {verificationToken.Token} !");

            //return RedirectToPage("/Security/Verification", new { verificationIdentifier = verificationToken.Identifier });
            return RedirectToPage("/User/Index");
        }



        private async Task<IEnumerable<SlotDto>> GetAllSlotsAsync(int campaignId, int testCenterId)
        {
            int slotDuration = 15; // 15 minutes
            DateTime startTime = DateTime.Today.AddHours(8); // 08:00
            DateTime endTime = DateTime.Today.AddHours(16); // 16:00

            List<SlotDto> allSlots = new List<SlotDto>();

            var campaign = await _unitOfWork.Campaigns.GetByIdAsync(SelectedCampaignId);
            var testCenter = await _unitOfWork.TestCenters.GetByIdAsync(SelectedTestCenterId);
            var examinations = await _unitOfWork.Examinations.GetByCampaignTestCenter(campaign, testCenter);

            DateTime runDate = campaign.From;
            if (runDate < DateTime.Now)
            {
                runDate = DateTime.Now;
            }
            DateTime endDate = campaign.To;

            while (runDate.Date < endDate.Date)
            {
                if (startTime.TimeOfDay <= runDate.TimeOfDay && runDate.TimeOfDay < endTime.TimeOfDay)
                {
                    allSlots.Add(new SlotDto
                    {
                        Time = runDate,
                        SlotsAvailable = testCenter.SlotCapacity - examinations
                                                                        .Where(_ => _.ExaminationAt == runDate)
                                                                        .ToList()
                                                                        .Count
                    });
                }
                runDate = runDate.AddMinutes(slotDuration);
            }

            return allSlots;
        }
    }
}
