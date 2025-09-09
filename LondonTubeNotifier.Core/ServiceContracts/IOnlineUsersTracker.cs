namespace LondonTubeNotifier.Core.ServiceContracts
{
    public interface IOnlineUsersTracker
    {
        /// <summary>
        /// Adds a new user connection to the tracker.
        /// </summary>
        /// <param name="userId">The unique identifier of the user.</param>
        /// <param name="connectionId">The unique identifier of the user's SignalR connection.</param>
        void AddUser(string userId, string connectionId);

        /// <summary>
        /// Removes a user's specific connection from the tracker.
        /// </summary>
        /// <param name="userId">The unique identifier of the user.</param>
        /// <param name="connectionId">The unique identifier of the connection to remove.</param>
        void RemoveUser(string userId, string connectionId);

        
    }
}
