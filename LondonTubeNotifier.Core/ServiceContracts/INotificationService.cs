using LondonTubeNotifier.Core.DTOs;

namespace LondonTubeNotifier.Core.ServiceContracts
{
    /// <summary>
    /// Sends notifications to users and broadcasts updates.
    /// </summary>
    public interface INotificationService
    {
        /// <summary>
        /// Notifies all subscribers of a particular line.
        /// </summary>
        Task NotifyLineSubscribersAsync(NotificationDto notificationDto, CancellationToken cancellationToken);

        /// <summary>
        /// Broadcasts the full status to all users (e.g., homepage updates).
        /// </summary>
        Task NotifyAllAsync(FullStatusNotificationDto notificationDto, CancellationToken cancellationToken);
    }
}

