using LondonTubeNotifier.Core.Domain.Entities;
using LondonTubeNotifier.Core.DTOs;

namespace LondonTubeNotifier.Core.ServiceContracts
{
    public interface INotificationService
    {
        Task NotifyLineSubscribersAsync(NotificationDto notificationDto, CancellationToken cancellationToken);
    }
}
