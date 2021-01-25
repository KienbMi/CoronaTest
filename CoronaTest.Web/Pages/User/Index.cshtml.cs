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

        [DisplayName("Name")]
        public string Fullname { get; set; }

        [DisplayName("Handynummer")]
        public string MobileNumber { get; set; }

        [DisplayName("SVNr")]
        public string SocialSecurityNumber { get; set; }

        public Participant Participant { get; set; }

        public Examination[] Examinations { get; set; }

        public int Id { get; set; }

        public IndexModel(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task OnGetAsync()
        {
            Id = 1;
            Participant = await _unitOfWork.Participants.GetByIdAsync(Id);

            if (Participant == null)
            {
                return;
            }

            Examinations = await _unitOfWork.Examinations.GetByParticipantIdAsync(Id);

            Fullname = $"{Participant.Firstname} {Participant.Lastname}";
            MobileNumber = Participant.Mobilephone;
            SocialSecurityNumber = Participant.SocialSecurityNumber;
        }

        public async Task<IActionResult> OnGetDeleteAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var examination = await _unitOfWork.Examinations.GetByIdAsync(id.Value);
            if (examination == null)
            {
                return NotFound();
            }

            _unitOfWork.Examinations.Remove(examination);
            await _unitOfWork.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
