namespace LondonTubeNotifier.WebApi.Dtos
{
    public class AuthenticationResponse
    {
        public Guid Id { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? FullName { get; set; }
        public bool PushNotifications { get; set; }
        public bool EmailNotifications { get; set; }
        public string AccessToken { get; set; } = string.Empty;
        public DateTime AccessTokenExpiration { get; set; }
    }
}

