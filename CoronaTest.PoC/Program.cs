using CoronaTest.Core.Contracts;
using CoronaTest.Core.Services;
using Microsoft.Extensions.Configuration;
using System;

namespace CoronaTest.PoC
{
    class Program
    {
        static void Main(string[] args)
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Environment.CurrentDirectory)
                .AddJsonFile("appsettings.json")
                .AddUserSecrets<TwilioSmsService>()
                .AddEnvironmentVariables()
                .Build();

            ISmsService smsSerivce = new TwilioSmsService(
                configuration["Twilio:AccountSid"], configuration["Twilio:AuthToken"]);

            string message = "Hello World from Twilio SMS service.";
            string to = "+4368181820423";

            smsSerivce.SendSms(to, message);
        }
    }
}
