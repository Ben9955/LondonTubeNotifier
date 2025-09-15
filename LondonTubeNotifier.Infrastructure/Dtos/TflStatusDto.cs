namespace LondonTubeNotifier.Infrastructure.Dtos
{
    public class TflStatusDto
    {
        public int StatusSeverity { get; set; }
        public string StatusSeverityDescription { get; set; } = string.Empty;
        public string? Reason { get; set; }
    }
}
