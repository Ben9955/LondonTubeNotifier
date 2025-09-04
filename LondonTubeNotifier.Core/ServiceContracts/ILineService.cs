using LondonTubeNotifier.Core.DTOs;

namespace LondonTubeNotifier.Core.ServiceContracts
{


    /// <summary>
    /// Provides operations for querying and managing lines.
    /// </summary>
    public interface ILineService
    {
        /// <summary>
        /// Gets all the lines.
        /// </summary>
        /// <returns>A task that returns a collection of <see cref="LineDto"/>.</returns>
        Task<IEnumerable<LineDto>> GetLinesAsync();

        /// <summary>
        /// Retrieves a line by its unique identifier.
        /// </summary>
        /// <param name="lineId">The unique identifier of the line.</param>
        /// <returns>
        /// A <see cref="LineDto"/> representing the line if found; otherwise, null.
        /// </returns>
        /// <exception cref="DomainValidationException">Thrown if <paramref name="lineId"/> is null, empty, or invalid.</exception>
        /// <exception cref="DomainNotFoundException">Thrown if no line exists with the specified <paramref name="lineId"/>.</exception>
        Task<LineDto?> GetLineByLineIdAsync(string lineId);
    }
}
