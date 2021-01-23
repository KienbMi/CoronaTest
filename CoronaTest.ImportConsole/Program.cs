using CoronaTest.Persistence;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace CoronaTest.ImportConsole
{
    class Program
    {
        static async Task Main(string[] args)
        {
            await InitDataAsync();

            Console.WriteLine();
            Console.Write("Beenden mit Eingabetaste ...");
            Console.ReadLine();
        }

        private static async Task InitDataAsync()
        {
            Console.WriteLine("***************************");
            Console.WriteLine("          Import");
            Console.WriteLine("***************************");
            Console.WriteLine("Import der Teststationen in die Datenbank");
            await using var unitOfWork = new UnitOfWork();
            Console.WriteLine("Datenbank löschen");
            await unitOfWork.DeleteDatabaseAsync();
            Console.WriteLine("Datenbank migrieren");
            await unitOfWork.MigrateDatabaseAsync();

            Console.WriteLine("Daten werden von csv-Dateien eingelesen");
            var campaigns = ImportController.ReadFromCsv().ToArray();
            Console.WriteLine($"  {campaigns.Count()} Impfkampagnen eingelesen");

            await unitOfWork.Campaigns.AddRangeAsync(campaigns);

            Console.WriteLine("Impfkampagnen werden in Datenbank gespeichert (persistiert)");
            await unitOfWork.SaveChangesAsync();

            var cntCampaigns = await unitOfWork.Campaigns.GetCountAsync();
            var cntTestCenters = await unitOfWork.TestCenters.GetCountAsync();

            Console.WriteLine($"  Es wurden {cntCampaigns} Kampangnen gespeichert!");
            Console.WriteLine($"  Es wurden {cntTestCenters} Testcenter gespeichert!");
        }
    }
}
