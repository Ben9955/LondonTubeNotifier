using LondonTubeNotifier.Core.DTOs;

namespace LondonTubeNotifier.Core.MapperContracts
{
    /// <summary>
    /// Maps domain/user subscription and line status data into a notification DTO ready for sending.
    /// </summary>
    public interface INotificationMapper
    {
        /// <summary>
        /// Converts a user's subscription info and a line's current status into a notification object.
        /// </summary>
        NotificationDto Map(UserSubscriptionDto user, LineStatusesDto lineStatuses);
    }
}
