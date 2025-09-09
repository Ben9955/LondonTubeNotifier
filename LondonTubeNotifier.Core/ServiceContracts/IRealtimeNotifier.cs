using LondonTubeNotifier.Core.DTOs;

namespace LondonTubeNotifier.Core.ServiceContracts
{
    public interface IRealtimeNotifier
    {
        Task NotifyUserAsync(string userId, NotificationDto notificationDto, CancellationToken cancellationToken);
    }
}
