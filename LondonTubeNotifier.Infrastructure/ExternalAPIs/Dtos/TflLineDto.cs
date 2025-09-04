namespace LondonTubeNotifier.Core.DTOs.TflDtos
{
    public class TflLineDto
    {
        public string LineId { get; set; } = string.Empty;
        public List<TflStatusDto> LineStatuses { get; set; } = new();
    }
}
