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
using CoronaTest.Web.DataTransferObjects;
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

        [BindProperty]
        public Guid VerificationIdentifier { get; set; }

        [BindProperty]
        public ReservationDto Reservation { get; set; }

        public ReservationModel(
            IUnitOfWork unitOfWork,
            ISmsService smsService,
            Randomizer randomizer)
        {
            _unitOfWork = unitOfWork;
            _smsService = smsService;
            _stringRandomizer = randomizer;
        }

        public async Task<IActionResult> OnGetAsync(Guid? verificationIdentifier, int? id)
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
            
            Reservation = new ReservationDto();

            if (id != null)
            {
                var examination = await _unitOfWork.Examinations.GetByIdAsync(id.Value);
                if (examination != null)
                {
                    Reservation.ExaminationId = examination.Id;
                    Reservation.SelectedCampaignId = examination.Campaign.Id;
                    Reservation.SelectedTestCenterId = examination.TestCenter.Id;
                    Reservation.SelectedDay = examination.ExaminationAt.Date;
                    Reservation.SelectedSlot = examination.ExaminationAt;
                }
            }

            await GetComboBoxItems();

            return Page();
        }

        public async Task OnPostAsync()
        {
            await GetComboBoxItems();
        }

        public async Task<IActionResult> OnPostSaveAsync()
        {
            await GetComboBoxItems();
            if (!ModelState.IsValid)
            {
                return Page();
            }

            if (Reservation.SelectedCampaignId <= 0)
            {
                ModelState.AddModelError($"{nameof(Reservation)}.{nameof(Reservation.SelectedCampaignId)}", $"Kampagne auswählen");
                return Page();
            }

            if (Reservation.SelectedTestCenterId <= 0)
            {
                ModelState.AddModelError($"{nameof(Reservation)}.{nameof(Reservation.SelectedTestCenterId)}", $"Untersuchungsort auswählen");
                return Page();
            }

            if (Reservation.SelectedDay == default(DateTime))
            {
                ModelState.AddModelError($"{nameof(Reservation)}.{nameof(Reservation.SelectedDay)}", $"Tag auswählen");
                return Page();
            }

            if (Reservation.SelectedSlot == default(DateTime))
            {
                ModelState.AddModelError($"{nameof(Reservation)}.{nameof(Reservation.SelectedSlot)}", $"Zeit auswählen");
                return Page();
            }

            var participant = (await _unitOfWork.VerificationTokens.GetTokenByIdentifierAsync(VerificationIdentifier)).Participant;

            Examination examination;

            if (Reservation.ExaminationId > 0)
            {
                examination = await _unitOfWork.Examinations.GetByIdAsync(Reservation.ExaminationId);
                examination.Campaign = await _unitOfWork.Campaigns.GetByIdAsync(Reservation.SelectedCampaignId);
                examination.TestCenter = await _unitOfWork.TestCenters.GetByIdAsync(Reservation.SelectedTestCenterId);
                examination.ExaminationAt = Reservation.SelectedDay.AddMinutes(Reservation.SelectedSlot.TimeOfDay.TotalMinutes);
            }
            else
            {
                examination = new Examination
                {
                    Campaign = await _unitOfWork.Campaigns.GetByIdAsync(Reservation.SelectedCampaignId),
                    Participant = await _unitOfWork.Participants.GetByIdAsync(participant.Id),
                    TestCenter = await _unitOfWork.TestCenters.GetByIdAsync(Reservation.SelectedTestCenterId),
                    Result = TestResult.Unknown,
                    State = ExaminationStates.New,
                    ExaminationAt = Reservation.SelectedDay.AddMinutes(Reservation.SelectedSlot.TimeOfDay.TotalMinutes),
                    Identifier = _stringRandomizer.Next()
                };
                await _unitOfWork.Examinations.AddAsync(examination);
            }

            try
            {
                await _unitOfWork.SaveChangesAsync();
            }
            catch (ValidationException ex)
            {
                ModelState.AddModelError("", $"{ex.Message}");
                return Page();
            }

            _smsService.SendSms(examination.Participant.Mobilephone, examination.GetReservationText());

            return RedirectToPage("/User/Index", new { verificationIdentifier = VerificationIdentifier });
        }

        public async Task GetComboBoxItems()
        {
            IEnumerable<SlotDto> allSlots = new List<SlotDto>();
            IEnumerable<SlotDto> availableSlots = new List<SlotDto>();

            var campaigns = await _unitOfWork.Campaigns.GetAllAsync();
            Reservation.Campaigns.AddRange(campaigns
                .Select(campaign => new SelectListItem
                                (campaign.Name, campaign.Id.ToString())));

            if (Reservation.SelectedCampaignId > 0)
            {
                var testCenters = await _unitOfWork.TestCenters.GetByCampaignIdAsync(Reservation.SelectedCampaignId);
                Reservation.TestCenters.AddRange(testCenters
                    .Select(testCenter => new SelectListItem
                                    (testCenter.Name, testCenter.Id.ToString())));
            }

            if (Reservation.SelectedTestCenterId > 0)
            {
                allSlots = await _unitOfWork.TestCenters.GetAllSlotsByCampaignIdAsync(Reservation.SelectedCampaignId, Reservation.SelectedTestCenterId);

                availableSlots = allSlots
                    .Where(_ => _.SlotsAvailable > 0)
                    .ToList();

                Reservation.Days.AddRange(availableSlots
                    .GroupBy(_ => _.Time.Date)
                    .Select(group => new SelectListItem
                                    (group.Key.Date.ToShortDateString(), group.Key.Date.ToString())));
            }

            if (Reservation.SelectedDay != default(DateTime))
            {
                Reservation.Slots.AddRange(availableSlots
                    .Where(_ => _.Time.Date == Reservation.SelectedDay)
                    .Select(slot => new SelectListItem
                    ($"{slot.Time.ToShortTimeString()} | Verfügbar: {slot.SlotsAvailable}", slot.Time.ToString())));
            }
        }
    }
}
