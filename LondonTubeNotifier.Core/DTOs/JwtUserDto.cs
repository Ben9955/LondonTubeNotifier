namespace LondonTubeNotifier.Core.DTOs
{
    public class JwtUserDto
    {
        public Guid Id { get; set; }
        public string UserName { get; set; } = string.Empty;
    }
}
