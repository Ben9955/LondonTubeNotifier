namespace LondonTubeNotifier.Core.DTOs.TflDtos
{
    public class LineStatusDto
    {
        public string Id { get; set; } = string.Empty;
        public List<LineStatusDetailDto> LineStatuses { get; set; } = new();
    }
}
