namespace LondonTubeNotifier.WebApi.Dtos
{
    public class AuthenticationResponse
    {
        public Guid Id { get; set; }
        public string? UserName { get; set; }
        public string? Email { get; set; }
        public string? FullName { get; set; }
        public string? AccessToken { get; set; }
        public string? RefreshToken { get; set; }
        public DateTime AccessTokenExpiration { get; set; }
    }
}

