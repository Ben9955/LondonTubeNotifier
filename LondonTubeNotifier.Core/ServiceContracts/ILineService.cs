using LondonTubeNotifier.Core.DTOs;

namespace LondonTubeNotifier.Core.ServiceContracts
{


    /// <summary>
    /// Represents business logic for manipulating Line entity
    /// </summary>
    public interface ILineService
    {
        /// <summary>
        /// Gets all the lines.
        /// </summary>
        /// <returns>Returns a list of Lines as LineResponseDTO</returns>
        Task<List<LineResponseDTO>> GetLinesAsync();

        /// <summary>
        /// Gets a line by its Id.
        /// </summary>
        /// <param name="lineId">The Id of the line</param>
        /// <returns>A LineResponseDTO if found</returns>
        /// <exception cref="DomainValidationException">Thrown when lineId is null or empty</exception>
        /// <exception cref="DomainNotFoundException">Thrown when line with specified Id does not exist</exception>
        Task<LineResponseDTO?> GetLineByLineIdAsync(string lineId);

    }
}
