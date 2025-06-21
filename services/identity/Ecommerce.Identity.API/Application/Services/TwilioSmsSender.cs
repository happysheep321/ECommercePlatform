using Ecommerce.Identity.API.Application.Interfaces;
using Ecommerce.Identity.API.Infrastructure.Configurations;
using Microsoft.Extensions.Options;
using Twilio;
using Twilio.Rest.Api.V2010.Account;

namespace Ecommerce.Identity.API.Application.Services
{
    public class TwilioSmsSender:ISmsSender
    {
        private readonly TwilioSettings settings;

        public TwilioSmsSender(IOptions<TwilioSettings> options)
        {
            settings = options.Value;
            TwilioClient.Init(settings.AccountSid, settings.AuthToken);
        }

        public async Task SendAsync(string phoneNumber, string message)
        {
            var msg = await MessageResource.CreateAsync(
                body: message,
                from: new Twilio.Types.PhoneNumber(settings.FromPhoneNumber),
                to: new Twilio.Types.PhoneNumber(phoneNumber)
            );
        }
    }
}
