using LondonTubeNotifier.Core.ServiceContracts;
using SendGrid.Helpers.Mail;
using SendGrid;
using Microsoft.Extensions.Options;
using LondonTubeNotifier.Core.Configuration;
using Microsoft.Extensions.Logging;
using LondonTubeNotifier.Core.DTOs;

namespace LondonTubeNotifier.Infrastructure.Services
{
    public class EmailService : IEmailService
    {
        private readonly ILogger<EmailService> _logger;
        private readonly SendGridSettings _sendGridSettings;
        private readonly SendGridClient _client;
        private readonly IEmailTemplateService _emailTemplateService;


        public EmailService(IOptions<SendGridSettings> options, ILogger<EmailService> logger, IEmailTemplateService emailTemplateService)
        {
            _logger = logger;
            _emailTemplateService = emailTemplateService;
            _sendGridSettings = options.Value;

            if (string.IsNullOrWhiteSpace(_sendGridSettings.ApiKey))
                throw new InvalidOperationException("SendGrid API key is missing.");
            if (string.IsNullOrWhiteSpace(_sendGridSettings.FromName))
                throw new InvalidOperationException("SendGrid From Name is missing.");
            if (string.IsNullOrWhiteSpace(_sendGridSettings.FromEmail))
                throw new InvalidOperationException("SendGrid From Email is missing.");

            var SGoptions = new SendGridClientOptions
            {
                ApiKey = _sendGridSettings.ApiKey,
            };

            if (_sendGridSettings.DataResidency == "eu")
            {
                SGoptions.Host = "eu.api.sendgrid.com";
            }

            _client = new SendGridClient(SGoptions);
        }
        public async Task SendEmailAsync(EmailRecipientDto to, EmailContentDto content, string subject)
        {
            var renderedHtml = await _emailTemplateService.RenderAsync("~/Templates/LineStatusTemplate.cshtml", content);
            var plainText = _emailTemplateService.GeneratePlainText(content);

            var fromAddress = new EmailAddress(_sendGridSettings.FromEmail, _sendGridSettings.FromName);
            var toAddress = new EmailAddress(to.Email, to.Name);

            var msg = MailHelper.CreateSingleEmail(fromAddress, toAddress, subject, plainText, renderedHtml);

            var response = await _client.SendEmailAsync(msg);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError($"Failed to send email.Status Code: {response.StatusCode}");
                var responseBody = await response.Body.ReadAsStringAsync();
                throw new Exception($"Failed to send email. Status Code: {response.StatusCode}. Response Body: {responseBody}");
            }
        }

    }
}
