using LondonTubeNotifier.Core.Domain.Entities;
using LondonTubeNotifier.Core.Domain.RespositoryContracts;
using LondonTubeNotifier.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace LondonTubeNotifier.Infrastructure.Repositories
{
    public class LineStatusRepository : ILineStatusRepository
    {
        private readonly ApplicationDbContext _dbContext;
        public LineStatusRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<HashSet<LineStatus>> GetLastStatusForLineAsync(string lineId, CancellationToken cancellationToken)
        {
            var statuses = await _dbContext.LineStatuses 
                .Where(ls => ls.LineId == lineId)
                .AsNoTracking()
                .ToListAsync(cancellationToken);

            return new HashSet<LineStatus>(statuses);
        }

        public async Task<Dictionary<string, HashSet<LineStatus>>> GetLatestLineStatusesAsync(CancellationToken cancellationToken)
        {
            var statuses = await _dbContext.LineStatuses
                .AsNoTracking()
                .ToListAsync(cancellationToken);
            return statuses.GroupBy(s => s.LineId)
                .ToDictionary(g => g.Key, g => g.ToHashSet());
        }

        public async Task UpdateLinesAsync(Dictionary<string, List<LineStatus>> updates, CancellationToken cancellationToken)
        {
            foreach (var kvp in updates)
            {
                var lineId = kvp.Key;
                var newStatuses = kvp.Value;

                var existingStatuses = _dbContext.LineStatuses.Where(s => s.LineId == lineId);
                _dbContext.LineStatuses.RemoveRange(existingStatuses);

                _dbContext.LineStatuses.AddRange(newStatuses);
            }

            await _dbContext.SaveChangesAsync();
        }

    }
}
