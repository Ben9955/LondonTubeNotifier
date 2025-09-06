using LondonTubeNotifier.Core.Domain.Interfaces;

namespace LondonTubeNotifier.Core.Domain.RespositoryContracts
{
    /// <summary>
    /// Handles data access for users.
    /// </summary>
    public interface IUserRepository
    {
        /// <summary>
        /// Retrieves a user along with their subscriptions.
        /// </summary>
        /// <param name="id">The unique identifier of the user.</param>
        /// <returns>The user with subscriptions if found; otherwise, null.</returns>
        Task<IUser?> GetUserWithSubscriptionsAsync(Guid id, CancellationToken cancellationToken);
    }
}
