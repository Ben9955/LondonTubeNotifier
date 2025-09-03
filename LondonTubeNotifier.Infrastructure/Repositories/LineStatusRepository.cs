using LondonTubeNotifier.Core.Domain.Entities;
using LondonTubeNotifier.Core.Domain.RespositoryContracts;
using LondonTubeNotifier.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace LondonTubeNotifier.Infrastructure.Repositories
{
    public class LineStatusRepository : ILineStatusRepository
    {
        private readonly  ApplicationDbContext _dbContext;
        public LineStatusRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<List<LineStatus>> GetLastStatusForLineAsync(string lineId)
        {
            return await _dbContext.LineStatuses
                .Where(ls => ls.LineId == lineId)
                .OrderByDescending(ls => ls.StatusSeverity)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<Dictionary<string, List<LineStatus>>> GetLatestLineStatusesAsync()
        {
            var statuses = await _dbContext.LineStatuses
                .AsNoTracking()
                .ToListAsync();
            return statuses.GroupBy(s => s.LineId)
                .ToDictionary(g => g.Key, g => g.ToList());
        }

        public async Task SaveStatusAsync(List<LineStatus> statuses)
        {

            using var transaction = await _dbContext.Database.BeginTransactionAsync();

            try
            {
                _dbContext.LineStatuses.RemoveRange(_dbContext.LineStatuses);
                await _dbContext.SaveChangesAsync();

                await _dbContext.LineStatuses.AddRangeAsync(statuses);
                await _dbContext.SaveChangesAsync();

                await transaction.CommitAsync();

            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                throw ;
            }
        }
    }
}
