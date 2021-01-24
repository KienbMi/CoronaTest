using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using CoronaTest.Core.Contracts;
using CoronaTest.Core.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CoronaTest.Web.Pages
{
    public class NewRegistrationModel : PageModel
    {
        private readonly IUnitOfWork _unitOfWork;

        [BindProperty]
        [Required(ErrorMessage = "Der {0} ist verpflichtend")]
        [DisplayName("Vorname")]
        public string Firstname { get; set; }

        [BindProperty]
        [Required(ErrorMessage = "Der {0} ist verpflichtend")]
        [DisplayName("Nachname")]
        public string Lastname { get; set; }

        [BindProperty]
        [Required(ErrorMessage = "Der {0} ist verpflichtend")]
        [DisplayName("Geburtstag")]
        public DateTime Birthdate { get; set; }

        [BindProperty]
        [Required(ErrorMessage = "Das {0} ist verpflichtend")]
        [DisplayName("Geschlecht")]
        public string Gender { get; set; }

        [BindProperty]
        [Required(ErrorMessage = "Die {0} ist verpflichtend")]
        [StringLength(10, ErrorMessage = "Die {0} muss genau 10 Zeichen lang sein!", MinimumLength = 10)]
        [DisplayName("Sozialversicherungsnummer")]
        public string SocialSecurityNumber { get; set; }

        [BindProperty]
        [Required(ErrorMessage = "Die {0} ist verpflichtend")]
        [StringLength(16, ErrorMessage = "Die {0} muss zw. {1} und {2} Zeichen lang sein!", MinimumLength = 5)]
        [DisplayName("Handynummer")]
        public string Mobilephone { get; set; }

        [BindProperty]
        [Required(ErrorMessage = "Die {0} ist verpflichtend")]
        [DisplayName("Straße")]
        public string Street { get; set; }

        [BindProperty]
        [Required(ErrorMessage = "Die {0} ist verpflichtend")]
        [DisplayName("Hausnummer")]
        public string HouseNumber { get; set; }

        [DisplayName("Stiege")]
        public string Stair { get; set; }

        [DisplayName("Tür")]
        public string Door { get; set; }

        [BindProperty]
        [Required(ErrorMessage = "Die {0} ist verpflichtend")]
        [DisplayName("PLZ")]
        public string Postalcode { get; set; }

        [BindProperty]
        [Required(ErrorMessage = "Die {0} ist verpflichtend")]
        [DisplayName("Ort")]
        public string City { get; set; }


        public NewRegistrationModel(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }


        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {            
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var participant = new Participant
            {
                Firstname = Firstname,
                Lastname = Lastname,
                Gender = Gender,
                SocialSecurityNumber = SocialSecurityNumber,
                Birthdate = Birthdate,
                Mobilephone = Mobilephone,
                Postalcode = Postalcode,
                City = City,
                Street = Street,
                HouseNumber = HouseNumber,
                Stair = Stair,
                Door = Door
            };

            await _unitOfWork.Participants.AddAsync(participant);

            try
            {
                await _unitOfWork.SaveChangesAsync();
            }
            catch(ValidationException ex)
            {
                ModelState.AddModelError("", $"{ex.Message}");
                return Page();
            }

            //_smsService.SendSms(Mobilenumber, $"CoronaTest - Token: {verificationToken.Token} !");

            //return RedirectToPage("/Security/Verification", new { verificationIdentifier = verificationToken.Identifier });
            return RedirectToPage("/Index");
        }


    }
}
