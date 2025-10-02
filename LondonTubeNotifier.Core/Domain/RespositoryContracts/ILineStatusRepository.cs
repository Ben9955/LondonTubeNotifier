using LondonTubeNotifier.Core.Domain.Entities;

namespace LondonTubeNotifier.Core.Domain.RespositoryContracts
{
    /// <summary>
    /// Manages storage and retrieval of line status information.
    /// </summary>
    public interface ILineStatusRepository
    {
        /// <summary>
        /// Returns all latest line statuses, grouped by line ID.
        /// </summary>
        /// <returns>
        /// A dictionary where the key is the line ID and the value is the set of statuses.
        /// Returns an empty dictionary if no statuses exist.
        /// </returns>
        Task<Dictionary<string, HashSet<LineStatus>>> GetLatestLineStatusesAsync(CancellationToken cancellationToken);

        /// <summary>
        /// Gets the most recent statuses for a specific line.
        /// </summary>
        /// <param name="lineId">The ID of the line.</param>
        /// <returns>
        /// A set of statuses for the line. Returns an empty list if the line has no statuses.
        /// </returns>
        Task<HashSet<LineStatus>> GetLastStatusForLineAsync(string lineId, CancellationToken cancellationToken);

        /// <summary>
        /// Saves the latest line statuses. Replaces existing records in a transactional way.
        /// </summary>
        /// <param name="statuses">
        /// A dictionary keyed by line ID, where each value is a list of statuses for that line.
        /// </param>
        /// <exception cref="Exception">Throws if saving to the database fails.</exception>
        Task UpdateLinesAsync(Dictionary<string, List<LineStatus>> statuses, CancellationToken cancellationToken);
    }
}
