using LondonTubeNotifier.Core.Domain.Entities;
using LondonTubeNotifier.Core.Domain.RespositoryContracts;
using LondonTubeNotifier.Core.DTOs;
using LondonTubeNotifier.Core.Exceptions;
using LondonTubeNotifier.Core.MapperContracts;
using LondonTubeNotifier.Core.ServiceContracts;
using Microsoft.Extensions.Logging;


namespace LondonTubeNotifier.Core.Services
{
    public class LineService : ILineService
    {
        private readonly ILineRepository _lineRepository;
        private readonly ILineMapper _lineMapper;
        private readonly ILogger<LineService> _logger;

        public LineService(ILineRepository lineRepository, ILineMapper lineMapper, ILogger<LineService> logger)
        {
            _lineRepository = lineRepository;
            _lineMapper = lineMapper;
            _logger = logger;
        }

        public async Task<LineResponseDTO?> GetLineByLineIdAsync(string lineId)
        {
            if (string.IsNullOrWhiteSpace(lineId))
            {
                _logger.LogWarning("Invalid lineId provided: {LineId}", lineId);
                throw new DomainValidationException("LineId cannot be null or empty", new[] { nameof(lineId) });
            }

            _logger.LogInformation("Fetching line with LineId: {LineId}", lineId);
            Line? line = await _lineRepository.GetLineByLineId(lineId);

            if (line == null)
            {
                _logger.LogDebug("Line not found. LineId: {LineId}", lineId);

                throw new DomainNotFoundException($"Line not found. LineId: {lineId}");
            }

            _logger.LogInformation("Line found: {LineId}", lineId);
            return _lineMapper.ToDto(line);

        }

        public async Task<List<LineResponseDTO>> GetLinesAsync()
        {
            List<Line> lines = await _lineRepository.GetLines();

            return _lineMapper.ToDtoList(lines);
        }
    }
}
