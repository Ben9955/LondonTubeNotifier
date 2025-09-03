using LondonTubeNotifier.Core.Domain.Entities;
using LondonTubeNotifier.Core.Domain.Interfaces;
using LondonTubeNotifier.Core.Domain.RespositoryContracts;
using Microsoft.Extensions.Caching.Memory;

namespace LondonTubeNotifier.Infrastructure.Repositories
{
    public class CachedLineRepository : ILineRepository
    {
        private readonly IMemoryCache _memoryCache;
        private readonly ILineRepository _innerRepository;
        private const string CacheKey = "LinesCache";
        public CachedLineRepository(IMemoryCache memoryCache, ILineRepository innerRepository)
        {
            _memoryCache = memoryCache;
            _innerRepository = innerRepository;
        }
        public async Task AddSubscriptionAsync(IUser user, Line line)
        {
            await _innerRepository.AddSubscriptionAsync(user, line);
        }

        public async Task DeleteSubscriptionAsync(IUser user, Line line)
        {
            await _innerRepository.DeleteSubscriptionAsync(user, line);
        }

        public async Task<Line?> GetLineByLineIdAsync(string lineId)
        {
            var allLines = await GetLinesAsync();
            return allLines.FirstOrDefault(l => l.Id == lineId);
        }

        public async Task<List<Line>> GetLinesAsync()
        {
            return await _memoryCache.GetOrCreateAsync(CacheKey, async entry =>
            await _innerRepository.GetLinesAsync()) ?? new List<Line>();
        }
    }
}
