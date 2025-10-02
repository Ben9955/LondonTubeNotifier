using LondonTubeNotifier.Core.ServiceContracts;
using Microsoft.Extensions.Logging;
using LondonTubeNotifier.Core.DTOs;

namespace LondonTubeNotifier.Infrastructure.Services
{
    public class EmailService : IEmailNotifier
    {
        private readonly ILogger<EmailService> _logger;
        private readonly IEmailTemplateService _templateService;
        private readonly IEmailSender _sender;



        public EmailService(ILogger<EmailService> logger, 
            IEmailTemplateService templateService, IEmailSender sender)
        {
            _logger = logger;
            _templateService = templateService;
            _sender = sender;
        }


        public async Task SendEmailAsync(NotificationDto dto, CancellationToken ct)
        {
            var html = await _templateService
                .RenderAsync("LondonTubeNotifier.Infrastructure.Templates.LineStatusTemplate.cshtml", dto);
            var plain = _templateService.GeneratePlainText(dto);
            var subject = GenerateSubject(dto);

            await _sender.SendAsync(dto.RecipientEmail, dto.RecipientName, subject, plain, html, ct);
        }

        private string GenerateSubject(NotificationDto dto)
        {
            var mainStatus = dto.LineUpdates.StatusDescriptions
                .OrderByDescending(s => s.StatusSeverity)
                .FirstOrDefault()?.StatusDescription;

            return $"Tube Update: {dto.LineUpdates.LineName} - {mainStatus}";
        }
    }
}
