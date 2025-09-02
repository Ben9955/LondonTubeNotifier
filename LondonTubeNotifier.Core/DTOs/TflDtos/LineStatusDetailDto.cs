namespace LondonTubeNotifier.Core.DTOs.TflDtos
{
    public class LineStatusDetailDto
    {
        public int StatusSeverity { get; set; }
        public string StatusSeverityDescription { get; set; } = string.Empty;
        public string? Reason { get; set; }
    }
}
