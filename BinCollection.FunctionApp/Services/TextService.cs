using Microsoft.Extensions.Logging;
using Twilio;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Types;

namespace BinCollection.FunctionApp.Services
{
    public class TextService : ITextService
    {
        private readonly string _accountSid = string.Empty;
        private readonly string _authToken = string.Empty;
        private readonly string _toPhoneNumber = string.Empty;
        private readonly ILogger<TextService> _logger;

        public TextService(string accountSid, string authToken, string toPhoneNumber, ILogger<TextService> logger)
        {
            _accountSid = accountSid ?? throw new ArgumentNullException();
            _authToken = authToken ?? throw new ArgumentNullException();
            _toPhoneNumber = toPhoneNumber ?? throw new ArgumentNullException();
            _logger = logger;
        }

        public void SendText(string textMessage)
        {
            try
            {
                TwilioClient.Init(_accountSid, _authToken);
                _ = MessageResource.Create(
                    body: textMessage,
                    from: new PhoneNumber("Bin Service"),
                    to: new PhoneNumber(_toPhoneNumber)
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending bin collection text");
            }
        }
    }
}
