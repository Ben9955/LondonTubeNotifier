using LondonTubeNotifier.Core.Domain.Entities;
using LondonTubeNotifier.Core.DTOs;

namespace LondonTubeNotifier.Core.MapperContracts
{
    public interface ILineMapper
    {
        LineResponseDTO ToDto(Line line);
        List<LineResponseDTO> ToDtoList(IEnumerable<Line> lines);
    }
}
