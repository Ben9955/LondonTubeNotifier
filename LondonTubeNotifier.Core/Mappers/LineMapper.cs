using LondonTubeNotifier.Core.Domain.Entities;
using LondonTubeNotifier.Core.DTOs;
using LondonTubeNotifier.Core.MapperContracts;

namespace LondonTubeNotifier.Core.Mappers
{
    public class LineMapper : ILineMapper
    {
        public LineDto ToDto(Line line)
        {
            return new LineDto
            {
                Id = line.Id,
                Code = line.Code,
                Name = line.Name,
                Color = line.Color,
                ModeName = line.ModeName,

    };
        }

        public List<LineDto> ToDtoList(IEnumerable<Line> lines)
        {
            return lines.Select(l => ToDto(l)).ToList();
        }
    }
}
