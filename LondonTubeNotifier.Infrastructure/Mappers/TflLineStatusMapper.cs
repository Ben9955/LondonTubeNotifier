using LondonTubeNotifier.Core.Domain.Entities;
using LondonTubeNotifier.Infrastructure.Dtos;

namespace LondonTubeNotifier.Infrastructure.Mappers
{
    public class TflLineStatusMapper : ITflLineStatusMapper
    {
        public LineStatus ToDomain(string lineId, TflStatusDto dto)
        {
            return new LineStatus
            {
                LineId = lineId,
                StatusSeverity = dto.StatusSeverity,
                StatusDescription = dto.StatusSeverityDescription,
                Reason = dto.Reason,
                CreatedAt = DateTimeOffset.UtcNow
            };
        }

        public IEnumerable<LineStatus> ToDomainList(string lineId, IEnumerable<TflStatusDto> dtos)
        {
            return dtos.Select(dto => ToDomain(lineId, dto));
        }
    }
}
