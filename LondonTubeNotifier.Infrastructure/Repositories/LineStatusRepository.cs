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
        public async Task<HashSet<LineStatus>> GetLastStatusForLineAsync(string lineId)
        {
            var statuses = await _dbContext.LineStatuses 
                .Where(ls => ls.LineId == lineId)
                .AsNoTracking()
                .ToListAsync();

            return new HashSet<LineStatus>(statuses);
        }

        public async Task<Dictionary<string, HashSet<LineStatus>>> GetLatestLineStatusesAsync()
        {
            var statuses = await _dbContext.LineStatuses
                .AsNoTracking()
                .ToListAsync();
            return statuses.GroupBy(s => s.LineId)
                .ToDictionary(g => g.Key, g => g.ToHashSet());
        }

        public async Task SaveStatusAsync(Dictionary<string, HashSet<LineStatus>> dicStatuses)
        {

            using var transaction = await _dbContext.Database.BeginTransactionAsync();

            try
            {
                List<LineStatus> statuses = dicStatuses.SelectMany(s => s.Value).ToList();

                _dbContext.LineStatuses.RemoveRange(_dbContext.LineStatuses);
                await _dbContext.SaveChangesAsync();

                await _dbContext.LineStatuses.AddRangeAsync(statuses);
                await _dbContext.SaveChangesAsync();

                await transaction.CommitAsync();

            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
    }
}
