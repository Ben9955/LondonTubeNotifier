using LondonTubeNotifier.Core.DTOs.TflDtos;

namespace LondonTubeNotifier.Core.ServiceContracts
{
    public interface ITflApiService
    {
        Task<List<LineStatusDto>?> GetLinesStatusAsync();
    }
}
