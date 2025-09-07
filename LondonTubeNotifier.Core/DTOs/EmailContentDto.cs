namespace LondonTubeNotifier.Core.DTOs
{
    public class EmailContentDto
    {
        public string RecipientName { get; set; } = string.Empty;
        public string LineName { get; set; } = string.Empty;
        public string NewStatus { get; set; } = string.Empty;
        public string NewStatusDescription { get; set; } = string.Empty;
        public string StatusCssClass { get; set; } = string.Empty;

    }
}


