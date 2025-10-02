using LondonTubeNotifier.Core.Domain.Entities;

namespace LondonTubeNotifier.Core.Domain.Interfaces
{
    public interface IUser
    {
        Guid Id { get; }
        public string? UserName { get; }
        public ICollection<Line> Subscriptions { get; set; }
        public string? FullName { get; set; } 
        public string? Email { get; set; }
        public bool PushNotifications { get; set; }
        public bool EmailNotifications { get; set; }
    }
}
