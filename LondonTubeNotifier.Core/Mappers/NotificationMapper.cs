using LondonTubeNotifier.Core.DTOs;
using LondonTubeNotifier.Core.MapperContracts;

namespace LondonTubeNotifier.Core.Mappers
{
    public class NotificationMapper : INotificationMapper
    {
        public NotificationDto Map(UserSubscriptionDto user, LineStatusesDto lineStatuses)
        {
            return new NotificationDto
            {
                RecipientEmail = user.Email,
                RecipientName = user.FullName,
                RecipientId = user.UserId,
                LineUpdates = lineStatuses
            };
        }
    }
}
