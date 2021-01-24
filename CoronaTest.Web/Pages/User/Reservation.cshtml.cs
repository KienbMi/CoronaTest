using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CoronaTest.Core.Contracts;
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
        public int SelectedTestCenter { get; set; }
        public List<SelectListItem> TestCenters { get; set; }
        public string ExaminationDay { get; set; }
        public string ExaminationTime { get; set; }

        
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
            Campaigns = new List<SelectListItem>{
                new SelectListItem(_cbDefaultText, _cbDefaultValue.ToString())};

            var campaigns = await _unitOfWork.Campaigns.GetAllAsync();
            Campaigns.AddRange(campaigns
                .Select(campaign => new SelectListItem
                                (campaign.Name, campaign.Id.ToString())));

            TestCenters = new List<SelectListItem>{
                new SelectListItem(_cbDefaultText, _cbDefaultValue.ToString())};


            if (SelectedCampaignId > 0)
            {
                var testCenters = await _unitOfWork.TestCenters.GetByCampaignIdAsync(SelectedCampaignId);
                TestCenters.AddRange(testCenters
                    .Select(testCenter => new SelectListItem
                                    (testCenter.Name, testCenter.Id.ToString())));
            }




        }

    }
}
