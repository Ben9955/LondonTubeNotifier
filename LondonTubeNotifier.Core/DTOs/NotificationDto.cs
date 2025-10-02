namespace LondonTubeNotifier.Core.DTOs
{

    public class StatusesDto
    {
        public string StatusDescription { get; set; } = string.Empty;
        public string? Reason { get; set; }
        public int StatusSeverity { get; set; }
        public string StatusCssClass { get; set; } = string.Empty ;
        public DateTimeOffset LastUpdate { get; set; }
    }
    public class LineStatusesDto
    {
        public string LineName { get; set; } = string.Empty;
        public string LineId { get; set; } = string.Empty;
        public List<StatusesDto> StatusDescriptions { get; set; } = new List<StatusesDto>();
    }

    public class NotificationDto
    {
        public string? RecipientName { get; set; }
        public string? RecipientEmail { get; set; }
        public string RecipientId { get; set; } = string.Empty;
        public LineStatusesDto LineUpdates { get; set; } = new LineStatusesDto(); 
    }
}
