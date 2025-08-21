using LondonTubeNotifier.Core.Domain.Entities;
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
        public async Task<List<Line>> GetLines()
        {
            return await _dbContext.Lines.AsNoTracking().ToListAsync();

        }

        public async Task<Line?> GetLineByLineId(string lineId)
        {
            return await _dbContext.Lines.FirstOrDefaultAsync(l => l.Id == lineId);
        }

    }
}
