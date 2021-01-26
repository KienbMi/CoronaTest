using CoronaTest.Core.Contracts;
using CoronaTest.Core.Models;
using CoronaTest.Core.Validations;
using CoronaTest.Web.DataTransferObjects;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace CoronaTest.Web.Pages.User
{
    public class DetailsModel : PageModel
    {
        private readonly IUnitOfWork _unitOfWork;
        [BindProperty]
        public Guid VerificationIdentifier { get; set; }
        [BindProperty]
        public ParticipantDto Participant { get; set; }

        public DetailsModel(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
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
            var participant = verificationToken.Participant;
            Participant = new ParticipantDto(participant);

            return Page();
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

            var participantInDb = await _unitOfWork.Participants.GetByIdAsync(Participant.Id);
            Participant.CopyContentToModel(ref participantInDb);

            try
            {
                await _unitOfWork.SaveChangesAsync();
            }
            catch (ValidationException ex)
            {
                ModelState.AddModelError("", $"{ex.Message}");
                return Page();
            }

            return RedirectToPage("/User/Index", new { verificationIdentifier = VerificationIdentifier } );
        }
    }
}
