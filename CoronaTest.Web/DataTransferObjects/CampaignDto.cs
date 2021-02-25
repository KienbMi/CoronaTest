using CoronaTest.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoronaTest.Web.DataTransferObjects
{
    public class CampaignDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime From { get; set; }
        public DateTime To { get; set; }

        public CampaignDto()
        {

        }

        public CampaignDto(Campaign campaign)
        {
            Id = campaign.Id;
            Name = campaign.Name;
            From = campaign.From;
            To = campaign.To;
        }
    }
}
