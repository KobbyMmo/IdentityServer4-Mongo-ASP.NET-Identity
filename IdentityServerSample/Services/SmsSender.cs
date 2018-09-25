using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Twilio;
using Twilio.Rest.Api.V2010.Account;

namespace IdentityServerSample.Services
{
    public class SmsSender : ISmsSender
    {
        public Task SendSmsAsync(string number, string message)
        {
            const string accountSid = "";
            const string authToken = "";

            TwilioClient.Init(accountSid, authToken);

            return MessageResource.CreateAsync(
                body: message,
                from: new Twilio.Types.PhoneNumber(""),
                to: new Twilio.Types.PhoneNumber(number)
            );
        }
    }
}
