using LondonTubeNotifier.Core.Domain.Entities;
using LondonTubeNotifier.Core.DTOs;

namespace LondonTubeNotifier.Core.MapperContracts
{
    /// <summary>
    /// Converts a set of <see cref="LineStatus"/> entities into a <see cref="LineStatusesDto"/> for client consumption.
    /// </summary>
    public interface ILineStatusToDtoMapper
    {
        /// <summary>
        /// Maps a line's statuses to a DTO.
        /// </summary>
        /// <param name="lineId">The ID of the line.</param>
        /// <param name="lineStatuses">The line's current statuses.</param>
        LineStatusesDto Map(string lineId, IEnumerable<LineStatus> lineStatuses);
    }
}
