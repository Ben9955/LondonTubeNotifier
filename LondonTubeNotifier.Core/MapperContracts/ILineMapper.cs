using LondonTubeNotifier.Core.Domain.Entities;
using LondonTubeNotifier.Core.DTOs;

namespace LondonTubeNotifier.Core.MapperContracts
{
    /// <summary>
    /// Converts <see cref="Line"/> domain entities into <see cref="LineDto"/> objects for use in the API or other layers.
    /// </summary>
    public interface ILineMapper
    {
        /// <summary>
        /// Maps a single Line entity to a LineDto.
        /// </summary>
        LineDto ToDto(Line line);

        /// <summary>
        /// Maps a collection of Line entities to a list of LineDto objects.
        /// </summary>
        List<LineDto> ToDtoList(IEnumerable<Line> lines);
    }
}
