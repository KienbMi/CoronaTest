using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using CoronaTest.Core.Contracts;
using CoronaTest.Core.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CoronaTest.Web.Pages.User
{
    public class IndexModel : PageModel
    {
        private readonly IUnitOfWork _unitOfWork;

        [BindProperty]
        public Guid VerificationIdentifier { get; set; }

        [DisplayName("Name")]
        public string Fullname { get; set; }

        [DisplayName("Handynummer")]
        public string MobileNumber { get; set; }

        [DisplayName("SVNr")]
        public string SocialSecurityNumber { get; set; }

        public Participant Participant { get; set; }

        public Examination[] Examinations { get; set; }

        public IndexModel(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IActionResult> OnGetAsync(Guid? verificationIdentifier)
        {          
            if(verificationIdentifier == null)
            {
                return RedirectToPage("/Security/TokenError");
            }
            
            VerificationToken verificationToken = await _unitOfWork.VerificationTokens.GetTokenByIdentifierAsync(verificationIdentifier.Value);

            if (verificationToken?.Identifier != verificationIdentifier || verificationToken.ValidUntil < DateTime.Now)
            {
                return RedirectToPage("/Security/TokenError");
            }

            VerificationIdentifier = verificationToken.Identifier;
            Participant = verificationToken.Participant;

            Examinations = await _unitOfWork.Examinations.GetByParticipantIdAsync(Participant.Id);

            Fullname = $"{Participant.Firstname} {Participant.Lastname}";
            MobileNumber = Participant.Mobilephone;
            SocialSecurityNumber = Participant.SocialSecurityNumber;

            return Page();
        }

        public async Task<IActionResult> OnGetDeleteAsync(int? id, Guid? verificationIdentifier)
        {
            if (id == null)
            {
                return NotFound();
            }

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

            var examination = await _unitOfWork.Examinations.GetByIdAsync(id.Value);
            if (examination == null)
            {
                return NotFound();
            }

            _unitOfWork.Examinations.Remove(examination);
            await _unitOfWork.SaveChangesAsync();

            return RedirectToPage("./Index", new { verificationIdentifier = VerificationIdentifier });
        }
    }
}
