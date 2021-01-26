using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using CoronaTest.Core.Contracts;
using CoronaTest.Core.Models;
using CoronaTest.Core.Validations;
using CoronaTest.Web.DataTransferObjects;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CoronaTest.Web.Pages
{
    public class RegistrationModel : PageModel
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ISmsService _smsService;

        [BindProperty]
        public ParticipantDto Participant { get; set; }

        public RegistrationModel(
            IUnitOfWork unitOfWork,
            ISmsService smsService)
        {
            _unitOfWork = unitOfWork;
            _smsService = smsService;
        }

        public void OnGet()
        {
            Participant = new ParticipantDto();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            if (!SSNrValidation.CheckSsnr(Participant.SocialSecurityNumber))
            {
                ModelState.AddModelError($"{nameof(Participant)}.{nameof(Participant.SocialSecurityNumber)}", $"Die SVNr {Participant.SocialSecurityNumber} ist ungültig");
                return Page();
            }

            var participant = Participant.GetNewModel();

            await _unitOfWork.Participants.AddAsync(participant);

            try
            {
                await _unitOfWork.SaveChangesAsync();
            }
            catch (ValidationException ex)
            {
                ModelState.AddModelError("", $"{ex.Message}");
                return Page();
            }

            VerificationToken verificationToken = VerificationToken.NewToken(participant);

            await _unitOfWork.VerificationTokens.AddAsync(verificationToken);
            await _unitOfWork.SaveChangesAsync();

            _smsService.SendSms(participant.Mobilephone, $"CoronaTest - Token: {verificationToken.Token} !");

            return RedirectToPage("/Security/Verification", new { verificationIdentifier = verificationToken.Identifier });
        }
    }
}
