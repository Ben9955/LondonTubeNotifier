using LondonTubeNotifier.Core.Domain.Entities;

namespace LondonTubeNotifier.Core.Domain.RespositoryContracts
{
    public interface ILineRepository
    {
        Task<List<Line>> GetLines();
        Task<Line?> GetLineByLineId(string lineId);
    }
}
