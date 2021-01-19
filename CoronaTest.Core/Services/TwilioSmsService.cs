using CoronaTest.Core.Contracts;
using System;
using System.Diagnostics;
using Twilio;
using Twilio.Rest.Api.V2010.Account;

namespace CoronaTest.Core.Services
{
    public class TwilioSmsService : ISmsService
    {
        private readonly string _accountSid;
        private readonly string _authToken;

        public TwilioSmsService(string accountSid, string authToken)
        {
            _accountSid = accountSid;
            _authToken = authToken;
        }

        public bool SendSms(string to, string message)
        {
            try
            {
                // Find your Account Sid and Token at twilio.com/console
                // and set the environment variables. See http://twil.io/secure
                string accountSid = _accountSid;
                string authToken = _authToken;

                TwilioClient.Init(accountSid, authToken);

                var sms = MessageResource.Create(
                    body: "Hello World from Twilio SMS service.",
                    from: new Twilio.Types.PhoneNumber("+16292091184"),
                    to: new Twilio.Types.PhoneNumber("+4368181820423")
                );
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return false;
            }

            return true;
        }
    }
}
