using LondonTubeNotifier.Core.Domain.Interfaces;
using LondonTubeNotifier.Core.DTOs;

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

        /// <summary>
        /// Retrieves all subscriptions for the given line IDs.
        /// Each result contains the LineId and the corresponding subscribed user.
        /// </summary>
        /// <param name="lineIds">The line IDs to fetch subscribers for.</param>
        /// <param name="cancellationToken">Token to cancel the async operation.</param>
        /// <returns>
        /// A flat list of <see cref="UserSubscriptionDto"/>, 
        /// where each item links a user to a subscribed line.
        /// </returns>
        Task<List<UserSubscriptionDto>> GetUsersByLineIdsAsync(IEnumerable<string> lineIds, CancellationToken cancellationToken);
    }
}
