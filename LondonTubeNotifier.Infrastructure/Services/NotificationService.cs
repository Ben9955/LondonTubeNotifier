using LondonTubeNotifier.Core.DTOs;
using LondonTubeNotifier.Core.ServiceContracts;
using Microsoft.Extensions.Logging;

namespace LondonTubeNotifier.Core.Services
{
    public class NotificationService : INotificationService
    {
        private readonly IUserOnlineChecker _onlineChecker;
        private readonly IEmailService _emailService;
        private readonly IRealtimeNotifier _realtimeNotifier;
        private readonly ILogger<NotificationService> _logger;
        public NotificationService(IUserOnlineChecker onlineChecker,IEmailService emailService, 
            IRealtimeNotifier realtimeNotifier, ILogger<NotificationService> logger)
        {
            _onlineChecker = onlineChecker;
            _emailService = emailService;
            _realtimeNotifier = realtimeNotifier;
            _logger = logger;
        }

        public async Task NotifyLineSubscribersAsync(NotificationDto notificationDto, CancellationToken cancellationToken)
        {
            var userId = notificationDto.RecipientId;

            if (_onlineChecker.IsUserOnline(userId))
            {
                _logger.LogDebug("Sending notifications for {LineName} to {UserName}",
                    notificationDto.LineUpdates.LineName, notificationDto.RecipientName);

                await _realtimeNotifier.NotifyUserAsync(userId, notificationDto, cancellationToken);
            }
            else
            {
                _logger.LogDebug("Sending notifications for {LineName} to {UserName}",
                    notificationDto.LineUpdates.LineName, notificationDto.RecipientName);

                await _emailService.SendEmailAsync(notificationDto, cancellationToken);
            }


            //_logger.LogInformation("Notifications for {LineName} have been sent to {UserCount} users",
            //    lineStatusesDto.LineName, users.Count);

            _logger.LogDebug("Notification for {LineName} to {UserName} has been sent",
                notificationDto.LineUpdates.LineName, notificationDto.RecipientName);
        }
    }
}
