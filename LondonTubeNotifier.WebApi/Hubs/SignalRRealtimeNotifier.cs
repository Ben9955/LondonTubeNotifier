using Microsoft.AspNetCore.SignalR;
using LondonTubeNotifier.Core.ServiceContracts;
using LondonTubeNotifier.Core.DTOs;

namespace LondonTubeNotifier.WebApi.Hubs
{
    public class SignalRRealtimeNotifier : IRealtimeNotifier
    {
        private readonly IHubContext<TflLiveHub> _hubContext;
        private readonly ILogger<SignalRRealtimeNotifier> _logger;

        public SignalRRealtimeNotifier(IHubContext<TflLiveHub> hubContext, ILogger<SignalRRealtimeNotifier> logger)
        {
            _hubContext = hubContext;
            _logger = logger;
        }

        public async Task NotifyUserAsync(string userId, NotificationDto notificationDto, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(userId)) return;
            
            var normalizedUserId = userId.ToUpperInvariant();
            
            _logger.LogInformation("Sending real-time notification to user {UserId}", normalizedUserId);

            // Send notification via SignalR
            await _hubContext.Clients.User(normalizedUserId)
                .SendAsync("ReceiveLineUpdate", notificationDto, cancellationToken);
        }

        public async Task NotifyAllAsync(FullStatusNotificationDto notificationDto, CancellationToken cancellationToken)
        {

            _logger.LogInformation("Broadcasting full status update to homepage group.");

            await _hubContext.Clients.Group(SignalRGroups.Homepage)
                .SendAsync("ReceiveFullStatusUpdate", notificationDto, cancellationToken);
        }

        public static class SignalRGroups
        {
            public const string Homepage = "homepage-group";
        }
    }

}