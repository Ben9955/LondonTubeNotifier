namespace LondonTubeNotifier.Core.DTOs
{
    public class UserSubscriptionDto
    {
        public string LineId { get; set; } = string.Empty;
        public string? FullName { get; set; }
        public string? Email { get; set; }
        public string UserId { get; set; } = string.Empty;

    }
}
