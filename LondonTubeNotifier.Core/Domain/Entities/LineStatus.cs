namespace LondonTubeNotifier.Core.Domain.Entities
{
    public class LineStatus
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string LineId { get; set; } = string.Empty;
        public Line Line { get; set; } = null!;
        public int StatusSeverity { get; set; }
        public string StatusDescription { get; set; } = string.Empty;
        public string? Reason { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
