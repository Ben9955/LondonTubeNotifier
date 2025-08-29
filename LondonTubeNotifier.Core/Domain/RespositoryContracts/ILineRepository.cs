using LondonTubeNotifier.Core.Domain.Entities;
using LondonTubeNotifier.Core.Domain.Interfaces;

namespace LondonTubeNotifier.Core.Domain.RespositoryContracts
{
    /// <summary>
    /// Handles data access for lines.
    /// </summary>
    public interface ILineRepository
    {
        /// <summary>
        /// Gets all lines.
        /// </summary>
        /// <returns>A list of all lines.</returns>
        Task<List<Line>> GetLinesAsync();

        /// <summary>
        /// Gets a line by its unique identifier.
        /// </summary>
        /// <param name="lineId">The identifier of the line.</param>
        /// <returns>The line if found; otherwise, null.</returns>
        Task<Line?> GetLineByLineIdAsync(string lineId);

        /// <summary>
        /// Adds a subscription of a user to a line.
        /// </summary>
        /// <param name="user">The user subscribing.</param>
        /// <param name="line">The line to subscribe to.</param>
        Task AddSubscriptionAsync(IUser user, Line line);

        /// <summary>
        /// Deletes a subscription of a user from a line.
        /// </summary>
        /// <param name="user">The user unsubscribing.</param>
        /// <param name="line">The line to unsubscribe from.</param>
        Task DeleteSubscriptionAsync(IUser user, Line line);
    }
}
