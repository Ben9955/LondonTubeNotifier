using LondonTubeNotifier.Core.Domain.Entities;
using LondonTubeNotifier.Core.DTOs;

namespace LondonTubeNotifier.Core.MapperContracts
{
    public interface ILineMapper
    {
        LineDto ToDto(Line line);
        List<LineDto> ToDtoList(IEnumerable<Line> lines);
    }
}
