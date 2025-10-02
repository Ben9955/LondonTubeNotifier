namespace LondonTubeNotifier.Infrastructure.Dtos
{
    public class TflLineDto
    {
        public string Id { get; set; } = string.Empty;
        public List<TflStatusDto> LineStatuses { get; set; } = new();
    }
}
