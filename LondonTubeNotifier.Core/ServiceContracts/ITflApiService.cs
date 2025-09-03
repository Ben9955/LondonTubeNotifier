using LondonTubeNotifier.Core.Domain.Entities;

namespace LondonTubeNotifier.Core.ServiceContracts
{
    /// <summary>
    /// Handles calls to the TfL API and returns line status data.
    /// </summary>
    public interface ITflApiService
    {
        /// <summary>
        /// Retrieves the latest status for all lines from the TfL API.
        /// </summary>
        /// <returns>A list of <see cref="LineStatus"/> representing the current status of all lines.</returns>
        /// <exception cref="TflApiException">Thrown if the TfL API request fails.</exception>
        /// <exception cref="JsonException">Thrown if the API response cannot be deserialized correctly.</exception>
        Task<Dictionary<string, List<LineStatus>>> GetLinesStatusAsync(CancellationToken cancellationToken);
    }
}
