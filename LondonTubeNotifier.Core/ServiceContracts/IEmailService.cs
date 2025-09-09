using LondonTubeNotifier.Core.DTOs;

namespace LondonTubeNotifier.Core.ServiceContracts
{
    public interface IEmailService
    {
        Task SendEmailAsync(NotificationDto notificationDto, CancellationToken cancellationToken);
    }
}
