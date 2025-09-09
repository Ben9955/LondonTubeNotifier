namespace LondonTubeNotifier.Core.ServiceContracts
{
    public interface IUserOnlineChecker
    {
        /// <summary>
        /// Checks if a user is currently online.
        /// </summary>
        /// <param name="userId">The unique identifier of the user.</param>
        /// <returns>True if the user has at least one active connection; otherwise, false.</returns>
        bool IsUserOnline(string userId);
    }
}