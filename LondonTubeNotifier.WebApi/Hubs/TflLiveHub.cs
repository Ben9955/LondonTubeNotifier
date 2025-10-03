using LondonTubeNotifier.Core.ServiceContracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace LondonTubeNotifier.WebApi.Hubs
{
    public class TflLiveHub : Hub
    {
        private readonly IOnlineUsersTracker _onlineUsersTracker;
        private readonly ILogger<TflLiveHub> _logger;
        public TflLiveHub(IOnlineUsersTracker onlineUsersTracker, ILogger<TflLiveHub> logger)
        {
            _onlineUsersTracker = onlineUsersTracker;
            _logger = logger;
        }

        [Authorize]
        public override async Task OnConnectedAsync()
        {
            var userId = Context.UserIdentifier?.ToUpperInvariant();

            if (!string.IsNullOrEmpty(userId))
            {
                _onlineUsersTracker.AddUser(userId, Context.ConnectionId);
                _logger.LogInformation("User {UserId} connected with ConnectionId {ConnectionId}", userId, Context.ConnectionId);
            }

            await base.OnConnectedAsync();
        }

        [Authorize]
        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            var userId = Context.UserIdentifier?.ToUpperInvariant();

            if (!string.IsNullOrEmpty(userId))
            {
                _onlineUsersTracker.RemoveUser(userId, Context.ConnectionId);
                _logger.LogInformation("User {UserId} disconnected with ConnectionId {ConnectionId}", userId, Context.ConnectionId);
            }

            await base.OnDisconnectedAsync(exception);
        }

        /// <summary>
        /// Allows a client to join a specific SignalR group.
        /// This is used by the client-side code when navigating to a page that requires real-time updates.
        /// </summary>
        /// <param name="groupName">The name of the group to join (e.g., "homepage-group").</param>
        [AllowAnonymous]
        public async Task JoinGroup(string groupName)
        {
            if (!string.IsNullOrEmpty(groupName))
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
            }
        }


        /// <summary>
        /// Allows a client to leave a specific SignalR group.
        /// This is used by the client-side code when navigating away from a page.
        /// </summary>
        /// <param name="groupName">The name of the group to leave.</param>
        [AllowAnonymous]
        public async Task LeaveGroup(string groupName)
        {
            if (!string.IsNullOrEmpty(groupName))
            {
                await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupName);
            }
        }

    }
}
