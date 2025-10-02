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
                Name = line.Name,
                Color = line.Color,
                ModeName = line.ModeName,
                StatusDescriptions = line.LineStatuses
                .OrderByDescending(ls => ls.CreatedAt)
                .Select(ls => new StatusesDto
                {
                    StatusSeverity = ls.StatusSeverity,
                    StatusDescription = ls.StatusDescription,
                    Reason = ls.Reason,
                    LastUpdate = ls.CreatedAt
                }).ToList()
                };
        }

        public List<LineDto> ToDtoList(IEnumerable<Line> lines)
        {
            return lines.Select(l => ToDto(l)).ToList();
        }
    }
}
