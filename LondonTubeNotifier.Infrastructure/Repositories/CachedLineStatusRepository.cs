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
        public async Task<HashSet<LineStatus>> GetLastStatusForLineAsync(string lineId, CancellationToken cancellationToken)
        {
            var statusesByLine = await GetLatestLineStatusesAsync(cancellationToken);

            if (statusesByLine.TryGetValue(lineId, out var lineStatuses))
            {
                return lineStatuses;
            }

            return new HashSet<LineStatus>();
        }

        public async Task<Dictionary<string, HashSet<LineStatus>>> GetLatestLineStatusesAsync(CancellationToken cancellationToken)
        {
            return await _memoryCache.GetOrCreateAsync(CacheKey, async entry =>
             {
                 entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5);

                 return await _innerRepository.GetLatestLineStatusesAsync(cancellationToken);
             }) ?? new Dictionary<string, HashSet<LineStatus>>();
        }

        public async Task UpdateLinesAsync(Dictionary<string, List<LineStatus>> updates, CancellationToken cancellationToken)
        {
            await _innerRepository.UpdateLinesAsync(updates, cancellationToken);

            if (!_memoryCache.TryGetValue<Dictionary<string, HashSet<LineStatus>>>(CacheKey, out var cachedStatuses))
            {
                cachedStatuses = await _innerRepository.GetLatestLineStatusesAsync(cancellationToken);
                _memoryCache.Set(CacheKey, cachedStatuses, new MemoryCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5)
                });

                return;
            }

            foreach (var kvp in updates)
            {
                var lineId = kvp.Key;
                var newStatuses = kvp.Value;
                cachedStatuses[lineId] = newStatuses.ToHashSet();
            }

            _memoryCache.Set(CacheKey, cachedStatuses, new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5)
            });
        }
    }
}
