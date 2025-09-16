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
            return await _dbContext.Lines.AsNoTracking().FirstOrDefaultAsync(l => l.Id == lineId);
        }

        public async Task AddSubscriptionAsync(IUser user, Line line)
        {
            if (_dbContext.Entry(line).State == EntityState.Detached)
            {
                _dbContext.Lines.Attach(line);
            }

            if (!user.Subscriptions.Any(s => s.Id == line.Id))
            {
                user.Subscriptions.Add(line);
            }

            await _dbContext.SaveChangesAsync();
        }
        
        public async Task DeleteSubscriptionAsync(IUser user, Line line)
        {
            var subscriptionToRemove = user.Subscriptions.FirstOrDefault(s => s.Id == line.Id);

            if (subscriptionToRemove != null)
            {
                user.Subscriptions.Remove(subscriptionToRemove);
                await _dbContext.SaveChangesAsync();
            }
        }

    }
}
