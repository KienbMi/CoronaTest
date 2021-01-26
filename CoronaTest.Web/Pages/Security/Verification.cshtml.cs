using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using CoronaTest.Core.Contracts;
using CoronaTest.Core.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CoroneTest.Web.Pages.Security
{
    public class VerificationModel : PageModel
    {
        private readonly IUnitOfWork _unitOfWork;

        [BindProperty]
        [Required(ErrorMessage = "Der Token ist verpflichtend")]
        [Range(100000, 999999, ErrorMessage= "Der Token muss 6 stellig sein!")]
        public int Token { get; set; }

        [BindProperty]
        public Guid VerificationIdentifier { get; set; }

        public VerificationModel(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IActionResult OnGet(Guid verificationIdentifier)
        {
            VerificationIdentifier = verificationIdentifier;

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            VerificationToken verificationToken = await _unitOfWork.VerificationTokens.GetTokenByIdentifierAsync(VerificationIdentifier);

            if(verificationToken.Token == Token && verificationToken.ValidUntil >= DateTime.Now)
            {
                return RedirectToPage("/Security/Success", new { verificationIdentifier = verificationToken.Identifier });
            }
            else
            {
                return RedirectToPage("/Security/TokenError");
            }
        } 
    }
}
