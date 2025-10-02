using SendGrid;
using SendGrid.Helpers.Mail;
using Microsoft.Extensions.Options;
using LondonTubeNotifier.Core.Configuration;
using LondonTubeNotifier.Core.ServiceContracts;
using Microsoft.Extensions.Logging;

namespace LondonTubeNotifier.Infrastructure.Services
{
    public class SendGridEmailSender : IEmailSender
    {
        private readonly SendGridClient _client;
        private readonly SendGridSettings _settings;
        private readonly ILogger<SendGridEmailSender> _logger;

        public SendGridEmailSender(IOptions<SendGridSettings> options, ILogger<SendGridEmailSender> logger)
        {
            _settings = options.Value;
            _logger = logger;

            if (string.IsNullOrWhiteSpace(_settings.ApiKey)) 
                throw new InvalidOperationException("SendGrid API key is missing."); 
            if (string.IsNullOrWhiteSpace(_settings.FromName)) 
                throw new InvalidOperationException("SendGrid From Name is missing."); 
            if (string.IsNullOrWhiteSpace(_settings.FromEmail)) 
                throw new InvalidOperationException("SendGrid From Email is missing.");

            var optionsClient = new SendGridClientOptions
            {
                ApiKey = _settings.ApiKey
            };

            if (_settings.DataResidency == "eu")
                optionsClient.Host = "eu.api.sendgrid.com";

            _client = new SendGridClient(optionsClient);
        }

        public async Task SendAsync(
            string toEmail,
            string toName,
            string subject,
            string plainText,
            string html,
            CancellationToken ct)
        {
            var from = new EmailAddress(_settings.FromEmail, _settings.FromName);
            var to = new EmailAddress(toEmail, toName);

            var msg = MailHelper.CreateSingleEmail(from, to, subject, plainText, html);
            var response = await _client.SendEmailAsync(msg, ct);

            if (!response.IsSuccessStatusCode)
            {
                var body = response.Body != null ? await response.Body.ReadAsStringAsync() : "";
                _logger.LogError("Failed to send email to {Email}. Status: {StatusCode}, Body: {Body}", toEmail, response.StatusCode, body);
                throw new Exception($"Failed to send email: {response.StatusCode}, {body}");
            }
        }
    }
}
