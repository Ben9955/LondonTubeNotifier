namespace LondonTubeNotifier.Core.Configuration
{
    public class TflSettings
    {
        public string ApiKey { get; set; } = string.Empty;
        public string BaseUrl { get; set; } = "https://api.tfl.gov.uk";
        public string Modes { get; set; } = "tube";
        public int PollingIntervalSeconds { get; set; } = 300;
    }
}
