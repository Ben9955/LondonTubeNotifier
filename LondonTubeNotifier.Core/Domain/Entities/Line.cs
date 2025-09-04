namespace LondonTubeNotifier.Core.Domain.Entities
{
    public class Line
    {
        public string Id { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Color { get; set; } = string.Empty;
        public string ModeName { get; set; } = string.Empty;
        public ICollection<LineStatus> LineStatuses { get; set; } = new List<LineStatus>();

    }
}
