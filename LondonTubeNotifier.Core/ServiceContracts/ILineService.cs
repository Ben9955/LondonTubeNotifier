using LondonTubeNotifier.Core.DTOs;

namespace LondonTubeNotifier.Core.ServiceContracts
{


    /// <summary>
    /// Provides business logic for managing lines.
    /// </summary>
    public interface ILineService
    {
        /// <summary>
        /// Gets all the lines.
        /// </summary>
        /// <returns>A task that returns a collection of <see cref="LineDto"/>.</returns>
        Task<IEnumerable<LineDto>> GetLinesAsync();

        /// <summary>
        /// Gets a line by its Id.
        /// </summary>
        /// <param name="lineId">The Id of the line</param>
        /// <returns>A task that returns a collection of <see cref="LineDto"/> for the given lineId, or null if not found.</returns>
        /// <exception cref="DomainValidationException">Thrown when lineId is null or empty</exception>
        /// <exception cref="DomainNotFoundException">Thrown when line with specified Id does not exist</exception>
        Task<LineDto?> GetLineByLineIdAsync(string lineId);
    }
}
