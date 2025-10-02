using LondonTubeNotifier.Core.DTOs;

namespace LondonTubeNotifier.Core.ServiceContracts
{
    /// <summary>
    /// Sends real-time notifications (e.g., WebSocket or SignalR) to users.
    /// </summary>
    public interface IRealtimeNotifier
    {
        /// <summary>
        /// Notifies a single user immediately.
        /// </summary>
        Task NotifyUserAsync(string userId, LineStatusesDto statusesDto, CancellationToken cancellationToken);

        /// <summary>
        /// Broadcasts notifications to all users in real-time.
        /// </summary>
        Task NotifyAllAsync(FullStatusNotificationDto notificationDto, CancellationToken cancellationToken);
    }
}
