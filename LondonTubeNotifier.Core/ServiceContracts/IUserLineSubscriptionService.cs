using LondonTubeNotifier.Core.DTOs;

namespace LondonTubeNotifier.Core.ServiceContracts
{
    /// <summary>
    /// Provides business logic for managing user subscriptions to lines.
    /// </summary>
    public interface IUserLineSubscriptionService
    {
        /// <summary>
        /// Subscribes a user to a specific line.
        /// </summary>
        /// <param name="userId">The unique identifier for the user.</param>
        /// <param name="lineId">The unique identifier for the line.</param>
        /// <exception cref="DomainValidationException">Thrown when the user is already subscribed to the given line</exception>
        /// <exception cref="DomainNotFoundException">Thrown when line and/or user with specified Id does not exist</exception>
        Task SubscribeAsync(Guid userId, string lineId);

        /// <summary>
        /// Unsubscribes a user from a specific line.
        /// </summary>
        /// <param name="userId">The unique identifier for the user.</param>
        /// <param name="lineId">The unique identifier for the line.</param>
        /// <exception cref="DomainValidationException">Thrown when the user is not subscribed to the given line</exception>
        /// <exception cref="DomainNotFoundException">Thrown when line and/or user with specified Id does not exist</exception>
        Task UnsubscribeAsync(Guid userId, string lineId);

        /// <summary>
        /// Gets all lines a user is subscribed to.
        /// </summary>
        /// <param name="userId">The unique identifier for the user.</param>
        /// <returns>A task that returns a collection of <see cref="LineDto"/> representing the user's subscriptions.</returns>
        /// <exception cref="DomainNotFoundException">Thrown when user with specified Id does not exist</exception>
        Task<IEnumerable<LineDto>> GetUserSubscriptionsAsync(Guid userId, CancellationToken cancellationToken);
    }
}
