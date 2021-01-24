using CoronaTest.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using Utils;

namespace CoronaTest.ImportConsole
{
    public class ImportController
    {
        const string FileNameTestCenters = "TestCenter.csv";
        const int Idx_TestCenterName = 0;
        const int Idx_TestCenterCity = 1;
        const int Idx_TestCenterPostalCode = 2;
        const int Idx_TestCenterStreet = 3;
        const int Idx_TestCenterSlotCapacity = 4;

        const string FileNameCampaigns = "Kampagnen.csv";
        const int Idx_CampaignName = 0;
        const int Idx_From = 1;
        const int Idx_To = 2;
        const int Idx_TestCenters = 3;


        public static IEnumerable<Campaign> ReadFromCsv()
        {
            string[][] matrix = MyFile.ReadStringMatrixFromCsv(FileNameTestCenters, true);

            //Name; Stadt; Plz; Strasse; SlotKapazität
            //Stadthalle Linz; Linz; 4020; Landstrasse 3; 7

            var testCenters = matrix.Select(tc => new TestCenter
            {
                Name = tc[Idx_TestCenterName],
                City = tc[Idx_TestCenterCity],
                Postalcode = tc[Idx_TestCenterPostalCode],
                Street = tc[Idx_TestCenterStreet],
                SlotCapacity = int.Parse(tc[Idx_TestCenterSlotCapacity])
                
            })
            .ToDictionary(tc => tc.Name);

            matrix = MyFile.ReadStringMatrixFromCsv(FileNameCampaigns, true);

            //Kampagnenname; Von; Bis; TestCenter
            //Antigentest Bildungsbereich OÖ; 01.10.2020; 14.10.2020; Stadthalle Linz, Stadthalle Leonding,Veranstaltungshalle Traun, Turnhalle Enns
            //Antigentest Bildungsbereich Wien; 03.10.2020; 24.10.2020; Stadthalle Wien, Turnhalle Wien,Veranstaltungshalle Wien

            var campaigns = matrix
                .Select(c => new Campaign
                {
                    Name = c[Idx_CampaignName], 
                    From = DateTime.Parse(c[Idx_From]),
                    To = DateTime.Parse(c[Idx_To]),
                    AvailableTestCenters = c[Idx_TestCenters]
                                    .Split(',')
                                    .Select(tc => testCenters[tc])
                                    .ToList()
                })
                .ToArray();

            //foreach (var campaign in campaigns)
            //{
            //    foreach (var testCenter in campaign?.AvailableTestCenters)
            //    {
            //        if (testCenter.AvailableInCampaigns == null)
            //        {
            //            testCenter.AvailableInCampaigns = new List<Campaign>();
            //        }
            //        testCenter.AvailableInCampaigns.Add(campaign);
            //    }
            //}

            return campaigns;
        }
    }
}
