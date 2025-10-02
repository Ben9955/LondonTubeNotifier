using System.Collections.Concurrent;
using LondonTubeNotifier.Core.ServiceContracts;

namespace LondonTubeNotifier.Core.Services
{
    public class OnlineUsersTracker : IOnlineUsersTracker, IUserOnlineChecker
    {
        private readonly ConcurrentDictionary<string, ConcurrentDictionary<string, byte>> _onlineUsers = new();
        public void AddUser(string userId, string connectionId)
        {

            _onlineUsers.AddOrUpdate(
                userId,
                new ConcurrentDictionary<string, byte>(
                    new[] { new KeyValuePair<string, byte>(connectionId, 0) }
                    ),
                (key, existingDic) =>
                {
                    existingDic.TryAdd(connectionId, 0);
                    return existingDic;
                });
        }

        public void RemoveUser(string userId, string connectionId)
        {
            if (_onlineUsers.TryGetValue(userId, out var connectionIds))
            {
                connectionIds.TryRemove(connectionId, out _);

                if (connectionIds.IsEmpty) _onlineUsers.TryRemove(userId, out _);
            }


        }

        public bool IsUserOnline(string userId)
        {
            return _onlineUsers.ContainsKey(userId);
        }
    }
}
