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
        public async Task SendEmailAsync(NotificationDto notificationDto, CancellationToken cancellationToken)
        {
            var renderedHtml = await _emailTemplateService.RenderAsync("~/Templates/LineStatusTemplate.cshtml", notificationDto);
            var plainText = _emailTemplateService.GeneratePlainText(notificationDto);

            var fromAddress = new EmailAddress(_sendGridSettings.FromEmail, _sendGridSettings.FromName);
            var toAddress = new EmailAddress(notificationDto.RecipientEmail, notificationDto.RecipientName);
            var subject = GenerateSubject(notificationDto);

            var msg = MailHelper.CreateSingleEmail(fromAddress, toAddress, subject, plainText, renderedHtml);

            var response = await SendEmailInternalAsync(msg, cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError($"Failed to send email.Status Code: {response.StatusCode}");
                var responseBody = response.Body != null
                    ? await response.Body.ReadAsStringAsync()
                    : string.Empty;
                throw new Exception($"Failed to send email. Status Code: {response.StatusCode}. Response Body: {responseBody}");
            }
        }

        private string GenerateSubject(NotificationDto dto)
        {
            var mainStatus = dto.LineUpdates.StatusDescriptions
                                .OrderByDescending(s => s.StatusSeverity)
                                .FirstOrDefault()?.StatusDescription;
            return $"Tube Update: {dto.LineUpdates.LineName} - {mainStatus}";
        }

        protected virtual Task<Response> SendEmailInternalAsync(SendGridMessage msg, CancellationToken cancellationToken)
        {
            return _client.SendEmailAsync(msg, cancellationToken);
        }


    }
}
