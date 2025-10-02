using LondonTubeNotifier.Core.Domain.Interfaces;
using LondonTubeNotifier.Core.Domain.RespositoryContracts;
using LondonTubeNotifier.Core.DTOs;
using LondonTubeNotifier.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace LondonTubeNotifier.Infrastructure.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly ApplicationDbContext _dbContext;
        public UserRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<IUser>> GetUsersByLineIdAsync(string lineId, CancellationToken cancellationToken)
        {
            return (await _dbContext.Users
                .AsNoTracking()
                .Where(u => u.Subscriptions.Any(l => l.Id == lineId))
                .ToListAsync(cancellationToken))
                .Cast<IUser>()
                .ToList();
        }

        public async Task<List<UserSubscriptionDto>> GetUsersByLineIdsAsync(IEnumerable<string> lineIds, CancellationToken cancellationToken)
        {
            return await _dbContext.Users
                .AsNoTracking()
                .Where(u => u.Subscriptions.Any(s => lineIds.Contains(s.Id))) 
                .SelectMany(
                u => u.Subscriptions.Where(l => lineIds.Contains(l.Id)),
                (user, subscription) => new UserSubscriptionDto
                {
                    LineId = subscription.Id,
                    FullName = user.FullName ?? user.UserName,
                    Email = user.Email,
                    UserId = user.Id.ToString()
                })
                .ToListAsync(cancellationToken);
        }

        public async Task<IUser?> GetUserWithSubscriptionsAsync(Guid id, CancellationToken cancellationToken)
        {
            var user = await _dbContext.Users
                .AsNoTracking()
                .Include(u => u.Subscriptions)
                .FirstOrDefaultAsync(u => u.Id == id, cancellationToken);

            return user;
        }
    }
}
