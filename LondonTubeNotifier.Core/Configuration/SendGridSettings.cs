namespace LondonTubeNotifier.Core.Configuration
{
    public class SendGridSettings
    {
        public string ApiKey { get; set; } = string.Empty;
        public string FromEmail { get; set; } = string.Empty;
        public string FromName { get; set; } = string.Empty;
        public string DataResidency { get; set; } = string.Empty;
    }
}
