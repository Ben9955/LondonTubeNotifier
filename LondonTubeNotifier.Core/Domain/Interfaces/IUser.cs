using LondonTubeNotifier.Core.Domain.Entities;

namespace LondonTubeNotifier.Core.Domain.Interfaces
{
    public interface IUser
    {
        Guid Id { get; }
        string UserName { get; }
        public ICollection<Line> Subscriptions { get; set; }
        public string? FullName { get; set; } 
        public string Email { get; set; } 
    }
}
