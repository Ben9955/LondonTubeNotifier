using LondonTubeNotifier.Core.Domain.Entities;
using LondonTubeNotifier.Core.Domain.RespositoryContracts;
using Microsoft.Extensions.Caching.Memory;

namespace LondonTubeNotifier.Infrastructure.Repositories
{
    public class CachedLineStatusRepository : ILineStatusRepository
    {
        private readonly IMemoryCache _memoryCache;
        private readonly ILineStatusRepository _innerRepository;
        private const string CacheKey = "LineStatusesCache";
        public CachedLineStatusRepository(IMemoryCache memoryCache, ILineStatusRepository innerRepository)
        {
            _memoryCache = memoryCache;
            _innerRepository = innerRepository;
        }
        public async Task<HashSet<LineStatus>> GetLastStatusForLineAsync(string lineId)
        {
            var statusesByLine = await GetLatestLineStatusesAsync();

            if (statusesByLine.TryGetValue(lineId, out var lineStatuses))
            {
                return lineStatuses;
            }

            return new HashSet<LineStatus>();
        }

        public async Task<Dictionary<string, HashSet<LineStatus>>> GetLatestLineStatusesAsync()
        {
            return await _memoryCache.GetOrCreateAsync(CacheKey, async entry =>
             {
                 entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5);

                 return await _innerRepository.GetLatestLineStatusesAsync();
             }) ?? new Dictionary<string, HashSet<LineStatus>>();
        }

        public async Task SaveStatusAsync(Dictionary<string, HashSet<LineStatus>> statuses)
        {
            await _innerRepository.SaveStatusAsync(statuses);

            _memoryCache.Set(CacheKey, statuses, new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(24)
            });
        }
    }
}
