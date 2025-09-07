using LondonTubeNotifier.Core.DTOs;

namespace LondonTubeNotifier.Core.ServiceContracts
{
    public interface IEmailService
    {
        Task SendEmailAsync(EmailRecipientDto to, EmailContentDto content, string subject);
    }
}
