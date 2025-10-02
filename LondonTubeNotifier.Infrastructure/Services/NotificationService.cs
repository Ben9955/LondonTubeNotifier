using LondonTubeNotifier.Core.DTOs;
using LondonTubeNotifier.Core.ServiceContracts;
using Microsoft.Extensions.Logging;

namespace LondonTubeNotifier.Core.Services
{
    public class NotificationService : INotificationService
    {
        private readonly IUserOnlineChecker _onlineChecker;
        private readonly IEmailNotifier _emailService;
        private readonly IRealtimeNotifier _realtimeNotifier;
        private readonly ILogger<NotificationService> _logger;
        public NotificationService(IUserOnlineChecker onlineChecker,IEmailNotifier emailService, 
            IRealtimeNotifier realtimeNotifier, ILogger<NotificationService> logger)
        {
            _onlineChecker = onlineChecker;
            _emailService = emailService;
            _realtimeNotifier = realtimeNotifier;
            _logger = logger;
        }

        // Sends user specific notifications
        public async Task NotifyLineSubscribersAsync(NotificationDto notificationDto, CancellationToken cancellationToken)
        {
            var userId = notificationDto.RecipientId.ToUpperInvariant();

            if (_onlineChecker.IsUserOnline(userId))
            {
                _logger.LogDebug("User online - Sending SignalR notification for {LineName} to {UserName}",
                    notificationDto.LineUpdates.LineName, notificationDto.RecipientName);

                await _realtimeNotifier.NotifyUserAsync(userId, notificationDto, cancellationToken);
            }
            else
            {
                _logger.LogDebug("User offline - Sending email for {LineName} to {UserName}",
                    notificationDto.LineUpdates.LineName, notificationDto.RecipientName);

                await _emailService.SendEmailAsync(notificationDto, cancellationToken);
            }

            _logger.LogDebug("Notification for {LineName} to {UserName} has been sent",
                notificationDto.LineUpdates.LineName, notificationDto.RecipientName);
        }

        // Broadcast changed line status to all connected clients
        public async Task NotifyAllAsync(FullStatusNotificationDto notificationDto, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Broadcasting changed line status to all connected clients.");
            await _realtimeNotifier.NotifyAllAsync(notificationDto, cancellationToken);
        }
    }
}
