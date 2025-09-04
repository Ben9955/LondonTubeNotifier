using LondonTubeNotifier.Core.Domain.Entities;
using LondonTubeNotifier.Core.Domain.Interfaces;
using LondonTubeNotifier.Core.Domain.RespositoryContracts;
using LondonTubeNotifier.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace LondonTubeNotifier.Infrastructure.Repositories
{
    public class LineRepository : ILineRepository
    {
        private readonly ApplicationDbContext _dbContext;
        public LineRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<List<Line>> GetLinesAsync()
        {
            return await _dbContext.Lines.AsNoTracking().ToListAsync();

        }

        public async Task<Line?> GetLineByLineIdAsync(string lineId)
        {
            return await _dbContext.Lines.FirstOrDefaultAsync(l => l.Id == lineId);
        }

        public async Task AddSubscriptionAsync(IUser user, Line line)
        {
            if (!user.Subscriptions.Contains(line))
            {
                user.Subscriptions.Add(line);
                await _dbContext.SaveChangesAsync();
            }
        }
        
        public async Task DeleteSubscriptionAsync(IUser user, Line line)
        {
            if(user.Subscriptions.Contains(line))
            {
                user.Subscriptions.Remove(line);
                await _dbContext.SaveChangesAsync();
            }
        }

    }
}
