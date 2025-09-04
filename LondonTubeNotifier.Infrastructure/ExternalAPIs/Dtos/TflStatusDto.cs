namespace LondonTubeNotifier.Core.DTOs.TflDtos
{
    public class TflStatusDto
    {
        public int StatusSeverity { get; set; }
        public string StatusSeverityDescription { get; set; } = string.Empty;
        public string? Reason { get; set; }
    }
}
