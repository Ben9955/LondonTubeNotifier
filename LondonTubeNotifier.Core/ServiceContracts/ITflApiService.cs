using LondonTubeNotifier.Core.DTOs.TflDtos;

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
        /// <returns>A list of <see cref="LineStatusDto"/> representing the current status of all lines.</returns>
        /// <exception cref="TflApiException">Thrown if the TfL API request fails.</exception>
        /// <exception cref="JsonException">Thrown if the API response cannot be deserialized correctly.</exception>
        /// <exception cref="Exception">Thrown for any other unexpected errors.</exception>
        Task<List<LineStatusDto>?> GetLinesStatusAsync();
    }
}
