using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CoronaTest.Core;
using CoronaTest.Core.Contracts;
using CoronaTest.Core.DataTransferObjects;
using CoronaTest.Core.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using StringRandomizer;

namespace CoronaTest.Web.Pages.User
{
    public class ReservationModel : PageModel
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ISmsService _smsService;
        private readonly Randomizer _stringRandomizer;
        private string _cbDefaultText = "Bitte auswählen";
        private int _cbDefaultValue = -1;

        [BindProperty]
        public Guid VerificationIdentifier { get; set; }

        [BindProperty]
        [Range(1, int.MaxValue, ErrorMessage="Bitte auswählen")]
        public int SelectedCampaignId { get; set; }
        public List<SelectListItem> Campaigns { get; set; }

        [BindProperty]
        [Range(1, int.MaxValue, ErrorMessage = "Bitte auswählen")]
        public int SelectedTestCenterId { get; set; }
        public List<SelectListItem> TestCenters { get; set; }

        [BindProperty]
        public DateTime SelectedDay { get; set; }
        public List<SelectListItem> Days { get; set; }

        [BindProperty]
        public DateTime SelectedSlot { get; set; }
        public List<SelectListItem> Slots { get; set; }


        public ReservationModel(
            IUnitOfWork unitOfWork,
            ISmsService smsService,
            Randomizer randomizer)
        {
            _unitOfWork = unitOfWork;
            _smsService = smsService;
            _stringRandomizer = randomizer;
        }

        public async Task<IActionResult> OnGetAsync(Guid? verificationIdentifier)
        {
            if (verificationIdentifier == null)
            {
                return RedirectToPage("/Security/TokenError");
            }

            VerificationToken verificationToken = await _unitOfWork.VerificationTokens.GetTokenByIdentifierAsync(verificationIdentifier.Value);

            if (verificationToken?.Identifier != verificationIdentifier || verificationToken.ValidUntil < DateTime.Now)
            {
                return RedirectToPage("/Security/TokenError");
            }

            VerificationIdentifier = verificationToken.Identifier;

            Campaigns = new List<SelectListItem>{
                new SelectListItem(_cbDefaultText, _cbDefaultValue.ToString())
                };

            var campaigns = await _unitOfWork.Campaigns.GetAllAsync();
            Campaigns.AddRange(campaigns
                .Select(campaign => new SelectListItem
                                (campaign.Name, campaign.Id.ToString())));

            return Page();
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
                    ($"{slot.Time.ToShortTimeString()} | Verfügbar: {slot.SlotsAvailable}", slot.Time.ToString())));
            }
        }

        public async Task<IActionResult> OnPostSaveAsync()
        {
            if(!ModelState.IsValid)
            {
                return Page();
            }

            var selectedDate = SelectedDay.AddMinutes(SelectedSlot.TimeOfDay.TotalMinutes);
            if (selectedDate < DateTime.Now)
            {
                ModelState.AddModelError("", $"Bitte auswählen");
                return Page();
            }

            var participant = (await _unitOfWork.VerificationTokens.GetTokenByIdentifierAsync(VerificationIdentifier)).Participant;

            var examination = new Examination
            {
                Campaign = await _unitOfWork.Campaigns.GetByIdAsync(SelectedCampaignId),
                Participant = await _unitOfWork.Participants.GetByIdAsync(participant.Id),
                TestCenter = await _unitOfWork.TestCenters.GetByIdAsync(SelectedTestCenterId),
                Result = TestResult.Unknown,
                State = ExaminationStates.New,
                ExaminationAt = selectedDate,
                Identifier = _stringRandomizer.Next()
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

            StringBuilder sb = new StringBuilder();
            sb.Append($"CoronaTest - Identifier: {examination.Identifier} ");
            sb.Append($"für Ihren Termin am {examination.ExaminationAt.ToShortDateString()} ");
            sb.Append($"um {examination.ExaminationAt.ToShortTimeString()} ");
            sb.Append($"im TestCenter: {examination.TestCenter.Name}!");

            _smsService.SendSms(examination.Participant.Mobilephone, sb.ToString());

            return RedirectToPage("/User/Index", new { verificationIdentifier = VerificationIdentifier });
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
