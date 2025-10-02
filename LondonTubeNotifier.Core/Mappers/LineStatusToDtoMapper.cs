using LondonTubeNotifier.Core.Domain.Entities;
using LondonTubeNotifier.Core.DTOs;
using LondonTubeNotifier.Core.MapperContracts;

namespace LondonTubeNotifier.Core.Mappers
{
    public class LineStatusToDtoMapper : ILineStatusToDtoMapper
    {
        public LineStatusesDto Map(string lineId, IEnumerable<LineStatus> lineStatuses)
        {
            var statusesDto = lineStatuses.Select(ls => new StatusesDto
            {
                StatusDescription = ls.StatusDescription,
                Reason = ls.Reason,
                StatusSeverity = ls.StatusSeverity,
                StatusCssClass = GetCssClassForStatus(ls.StatusSeverity),
                LastUpdate = ls.CreatedAt
            }).ToList();

            return new LineStatusesDto
            {
                LineName = Capitalize(lineId),
                LineId = lineId,
                StatusDescriptions = statusesDto
            };
        }

        private string GetCssClassForStatus(int severity)
        {
            // Switch to map severity to a CSS class
            return severity switch
            {
                >= 10 => "severe-delays",
                >= 6 => "minor-delays",
                _ => "good-service",
            };
        }

        private string Capitalize(string input)
        {
            if (string.IsNullOrEmpty(input)) return input;
            return char.ToUpper(input[0]) + input.Substring(1);
        }
    }
}
