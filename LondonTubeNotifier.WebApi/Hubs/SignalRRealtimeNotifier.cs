using Microsoft.AspNetCore.SignalR;
using LondonTubeNotifier.Core.ServiceContracts;
using LondonTubeNotifier.Core.DTOs;

namespace LondonTubeNotifier.WebApi.Hubs
{
    public class SignalRRealtimeNotifier : IRealtimeNotifier
    {
        private readonly IHubContext<TflLiveHub> _hubContext;

        public SignalRRealtimeNotifier(IHubContext<TflLiveHub> hubContext)
        {
            _hubContext = hubContext;
        }

        public async Task NotifyUserAsync(string userId, NotificationDto notificationDto, CancellationToken cancellationToken)
        {
            await _hubContext.Clients.User(userId).SendAsync("ReceiveLineUpdate", notificationDto, cancellationToken);
        }
    }

}