using LondonTubeNotifier.Core.Domain.Entities;
using LondonTubeNotifier.Infrastructure.Dtos;

namespace LondonTubeNotifier.Infrastructure.Mappers
{
    /// <summary>
    /// Converts raw TfL API data into domain-level <see cref="LineStatus"/> entities.
    /// </summary>
    public interface ITflLineStatusMapper
    {
        /// <summary>
        /// Maps a single TfL API status DTO into a domain LineStatus.
        /// </summary>
        /// <param name="lineId">The line ID the status belongs to.</param>
        /// <param name="dto">The TfL status DTO.</param>
        LineStatus ToDomain(string lineId, TflStatusDto dto);

        /// <summary>
        /// Maps a collection of TfL API status DTOs into domain LineStatus entities.
        /// </summary>
        /// <param name="lineId">The line ID the statuses belong to.</param>
        /// <param name="dtos">Collection of TfL status DTOs.</param>
        IEnumerable<LineStatus> ToDomainList(string lineId, IEnumerable<TflStatusDto> dtos);
    }
}
