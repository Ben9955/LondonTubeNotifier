namespace LondonTubeNotifier.Core.ServiceContracts
{
    public interface IOnlineUsersTracker
    {
        /// <summary>
        /// Adds a new user connection to the tracker.
        /// </summary>
        /// <param name="userId">The unique identifier of the user.</param>
        /// <param name="connectionId">The unique identifier of the user's SignalR connection.</param>
        public void AddUser(string userId, string connectionId);

        /// <summary>
        /// Removes a user's specific connection from the tracker.
        /// </summary>
        /// <param name="userId">The unique identifier of the user.</param>
        /// <param name="connectionId">The unique identifier of the connection to remove.</param>
        public void RemoveUser(string userId, string connectionId);

        /// <summary>
        /// Checks if a user is currently online.
        /// </summary>
        /// <param name="userId">The unique identifier of the user.</param>
        /// <returns>True if the user has at least one active connection; otherwise, false.</returns>
        public bool IsUserOnline(string userId);
    }
}
