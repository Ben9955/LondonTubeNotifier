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

        public async Task SaveStatusAsync(Dictionary<string, HashSet<LineStatus>> dicStatuses, CancellationToken cancellationToken)
        {

            using var transaction = await _dbContext.Database.BeginTransactionAsync(cancellationToken);

            try
            {
                List<LineStatus> statuses = dicStatuses.SelectMany(s => s.Value).ToList();

                _dbContext.LineStatuses.RemoveRange(_dbContext.LineStatuses);

                await _dbContext.LineStatuses.AddRangeAsync(statuses, cancellationToken);
                await _dbContext.SaveChangesAsync(cancellationToken);

                await transaction.CommitAsync(cancellationToken);

            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
    }
}
