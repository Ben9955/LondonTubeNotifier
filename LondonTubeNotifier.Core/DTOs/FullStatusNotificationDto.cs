using System.Collections.Generic;

namespace LondonTubeNotifier.Core.DTOs
{
    public class FullStatusNotificationDto
    {
        public List<LineStatusesDto> UpdatedLines { get; set; }
    }
}