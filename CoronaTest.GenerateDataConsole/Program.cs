using CoronaTest.Persistence;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace CoronaTest.GenerateDataConsole
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
            Console.WriteLine("Import der Untersuchungen in die Datenbank");
            await using var unitOfWork = new UnitOfWork();

            Console.WriteLine("Daten werden generiert");
            var examinations = (await GenerateData.GetExaminationsAsync(unitOfWork)).ToArray();
            Console.WriteLine($"  {examinations?.Count()} Untersuchungen generiert");

            await unitOfWork.Examinations.AddRangeAsync(examinations);

            Console.WriteLine("Untersuchungen werden in Datenbank gespeichert (persistiert)");
            await unitOfWork.SaveChangesAsync();

            var cntExaminations = examinations?.Count();

            Console.WriteLine($"  Es wurden {cntExaminations} Untersuchungen gespeichert!");
        }
    }
}
