using LondonTubeNotifier.Core.Domain.Entities;
using LondonTubeNotifier.Core.Domain.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace LondonTubeNotifier.Infrastructure.Entities
{
    public class ApplicationUser : IdentityUser<Guid>
    {
        public string? FullName { get; set; }
        public string? RefreshToken { get; set; }
        public DateTime? RefreshTokenExpiration { get; set; }
        public ICollection<Line> Subscriptions { get; set; } = new List<Line>();
    }
}
