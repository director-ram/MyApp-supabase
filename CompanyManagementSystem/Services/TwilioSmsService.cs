using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Twilio;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Types;

namespace CompanyManagementSystem.Services
{
    public class TwilioSmsService : ISmsService
    {
        private readonly string _accountSid;
        private readonly string _authToken;
        private readonly string _fromNumber;
        private readonly ILogger<TwilioSmsService> _logger;

        public TwilioSmsService(IConfiguration configuration, ILogger<TwilioSmsService> logger)
        {
            _logger = logger;
            _accountSid = configuration["Twilio:AccountSid"] ?? throw new InvalidOperationException("Twilio AccountSid not found in configuration");
            _authToken = configuration["Twilio:AuthToken"] ?? throw new InvalidOperationException("Twilio AuthToken not found in configuration");
            _fromNumber = configuration["Twilio:FromNumber"] ?? throw new InvalidOperationException("Twilio FromNumber not found in configuration");

            _logger.LogInformation("TwilioSmsService initialized with FromNumber: {FromNumber}", _fromNumber);
        }

        public async Task SendSmsAsync(string toNumber, string message)
        {
            try
            {
                _logger.LogInformation("Initializing Twilio client for sending SMS to {ToNumber}", toNumber);
                TwilioClient.Init(_accountSid, _authToken);

                // Ensure 'to' number is prefixed with 'whatsapp:' if sending from a WhatsApp number
                string normalizedToNumber = toNumber;
                if (_fromNumber.StartsWith("whatsapp:") && !toNumber.StartsWith("whatsapp:"))
                {
                    normalizedToNumber = $"whatsapp:{toNumber.Trim()}";
                    _logger.LogInformation("Normalized 'to' number for WhatsApp: {NormalizedToNumber}", normalizedToNumber);
                }

                _logger.LogInformation("Creating SMS message from {FromNumber} to {ToNumber}", _fromNumber, normalizedToNumber);
                var messageResource = await MessageResource.CreateAsync(
                    to: new PhoneNumber(normalizedToNumber),
                    from: new PhoneNumber(_fromNumber),
                    body: message
                );

                _logger.LogInformation("SMS sent successfully. Message SID: {MessageSid}", messageResource.Sid);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send SMS to {ToNumber}. Error: {Error}", toNumber, ex.Message);
                throw new Exception($"Failed to send SMS: {ex.Message}", ex);
            }
        }
    }
} 