using LondonTubeNotifier.Core.Domain.Entities;
using LondonTubeNotifier.Core.DTOs;
using LondonTubeNotifier.Core.MapperContracts;

namespace LondonTubeNotifier.Core.Mappers
{
    public class LineMapper : ILineMapper
    {
        public LineResponseDTO ToDto(Line line)
        {
            return new LineResponseDTO
            {
                Id = line.Id,
                Code = line.Code,
                Name = line.Name,
                Color = line.Color
            };
        }

        public List<LineResponseDTO> ToDtoList(IEnumerable<Line> lines)
        {
            return lines.Select(l => ToDto(l)).ToList();
        }
    }
}
