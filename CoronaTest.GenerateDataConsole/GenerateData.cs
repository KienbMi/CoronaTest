using CoronaTest.Core;
using CoronaTest.Core.Contracts;
using CoronaTest.Core.Models;
using CoronaTest.Persistence;
using StringRandomizer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoronaTest.GenerateDataConsole
{
    class GenerateData
    {
        const int MaxAmoutExaminations = 10000;
        static Random _random = new Random(DateTime.Now.Millisecond);
        static Randomizer _randomizer = new Randomizer();

        static (string name, string gender)[] _firstNames =
        {
            ("Andreas", "m"),
            ("Michael", "m"),
            ("Markus", "m"),
            ("Fritz", "m"),
            ("Daniel", "m"),
            ("Wolfgang", "m"),
            ("Josef", "m"),
            ("Peter", "m"),
            ("Alois", "m"),
            ("Werner", "m"),
            ("Andrea", "w"),
            ("Michaela", "w"),
            ("Marta", "w"),
            ("Frederike", "w"),
            ("Daniela", "w"),
            ("Marion", "w"),
            ("Maria", "w"),
            ("Petra", "w"),
            ("Alexandra", "w"),
            ("Marion", "w")
        };

        static string[] _lastNames =
        {
            "Mayr",
            "Huber",
            "Berger",
            "Bauer",
            "Müller",
            "Wagner",
            "Richter",
            "Schneider",
            "Fischer",
            "Wolf",
            "Zimmermann",
            "Hofmann",
            "Hartmann",
            "Becker",
            "Schmidt",
            "Schäfer",
            "Schröder",
            "Neumann",
            "Schulz",
            "Fuchs",
        };

        static string[] _streetNames =
        {
            "Arnleitnerweg",
            "Boltzmannstrasse",
            "Donatusgasse",
            "Einsteinstrasse",
            "Gabelsbergergang",
            "Gerstnerstrasse",
            "Hagenstrasse",
            "Hofmeindlweg",
            "Jahnstrasse",
            "Kokoweg",
            "Leithenfeldweg",
            "Marktplatz",
            "Neubauzeile",
            "Pfeifferstrasse",
            "Reindlstrasse",
            "Salesianumweg",
            "Schottweg",
            "Siemensstrasse",
            "Steinbauerstrasse",
            "Tobersbergerweg",
        };

        static (string name, string postalCode)[] _cities =
{
            ("Linz", "4020"),
            ("Leonding", "4060"),
            ("Wien", "1030"),
            ("Karlsdorf", "2052"),
            ("Eggendorf", "2604"),
            ("Scheibs", "3270"),
            ("Allhartsberg", "3365"),
            ("Sichersdorf", "4172"),
            ("Hochtor", "4322"),
            ("Thernberg", "4453"),
            ("Eck", "4742"),
            ("Köstendorf", "5203"),
            ("Bad Gastein", "5640"),
            ("Zell am See", "5700"),
            ("Hinterglemm", "5754"),
            ("Westendorf", "6363"),
            ("Silz", "6424"),
            ("Fließ", "6521"),
            ("Antau", "7042"),
            ("Kumberg", "8062")
        };


        public static async Task<IEnumerable<Examination>> GetExaminationsAsync(IUnitOfWork uow)
        {
            var campains = await uow.Campaigns.GetAllAsync();

            var examinations = new List<Examination>();

            for (int i = 0; i < MaxAmoutExaminations; i++)
            {
                var rndParticipant = GetParticipant();

                var rndCampaign = campains[_random.Next(campains.Length)];
                
                if (rndCampaign.AvailableTestCenters.Count == 0)
                    continue;

                var rndTestCenter = rndCampaign.AvailableTestCenters.ElementAt(_random.Next(rndCampaign.AvailableTestCenters.Count));

                var allSlots = await uow.TestCenters.GetAllSlotsByCampaignIdAsync(rndCampaign.Id, rndTestCenter.Id);

                var availableSlots = allSlots
                    .Where(_ => _.SlotsAvailable > 0)
                    .ToList();

                if (availableSlots.Count == 0)
                    continue;

                var rndSlot = availableSlots[_random.Next(availableSlots.Count)];

                TestResult rndResult;
                switch (_random.Next(7))
                {
                    case 0:
                        rndResult = TestResult.Positive;
                        break;
                    case 1:
                    case 2:
                    case 3:
                    case 4:
                        rndResult = TestResult.Negative;
                        break;
                    default:
                        rndResult = TestResult.Unknown;
                        break;
                }

                var rndState = (rndResult == TestResult.Unknown) ? ExaminationStates.New : ExaminationStates.Tested;

                var examination = new Examination
                {
                    Campaign = rndCampaign,
                    Participant = rndParticipant,
                    TestCenter = rndTestCenter,
                    Result = rndResult,
                    State = rndState,
                    ExaminationAt = rndSlot.Time,
                    Identifier = _randomizer.Next()
                };
                examinations.Add(examination);
            }

            return examinations;
        }

        private static Participant GetParticipant()
        {
            var participant = new Participant();

            var firstName = _firstNames[_random.Next(_firstNames.Length)];
            participant.Firstname = firstName.name;
            participant.Lastname = _lastNames[_random.Next(_lastNames.Length)];
            participant.Gender = (firstName.gender == "m") ? "männlich" : "weiblich";
            participant.Birthdate = DateTime.Now.Subtract(TimeSpan.FromDays(_random.Next(7300, 29000))).Date;
            participant.SocialSecurityNumber = GetSocialSecurityNumber(participant.Birthdate);
            participant.Mobilephone = $"+436{_random.Next(100000000, 999999999)}";
            var city = _cities[_random.Next(_cities.Length)];
            participant.Postalcode = city.postalCode;
            participant.City = city.name;
            participant.Street = _streetNames[_random.Next(_firstNames.Length)];
            participant.HouseNumber = $"{_random.Next(1, 99)}";
            participant.Stair = $"{_random.Next(1, 5)}";
            participant.Door = $"{_random.Next(1, 40)}";

            return participant;
        }

        private static string GetSocialSecurityNumber(DateTime birthdate)
        {
            string birthDateDigits = $"{ birthdate.Day:d2}{birthdate.Month:d2}{birthdate.Year % 100}";
            string firstThreeDigits;
            string checkDigit = "_";
            string ssnr;
            bool isValid;

            do
            {
                firstThreeDigits = $"{_random.Next(100, 1000)}";

                var digitweights = new int[] { 3, 7, 9, 0, 5, 8, 4, 2, 1, 6 };
                int productSum = 0;

                ssnr = $"{firstThreeDigits}{checkDigit}{birthDateDigits}";
                for (int i = 0; i < ssnr.Length; i++)
                {
                    productSum += (ssnr[i] - '0') * digitweights[i];
                }

                var checkdigit = productSum % 11;

                if (checkdigit < 10)
                {
                    isValid = true;
                    checkDigit = $"{checkdigit}";
                    ssnr = $"{firstThreeDigits}{checkDigit}{birthDateDigits}";
                }
                else
                {
                    isValid = false;
                }
            } while (!isValid);

            return ssnr;
        }
    }
}
