using LondonTubeNotifier.Core.DTOs;
using LondonTubeNotifier.Core.ServiceContracts;
using Microsoft.AspNetCore.Mvc;

namespace LondonTubeNotifier.WebApi.Controllers
{
    /// <summary>
    /// Handles operations related to lines.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class LinesController : ControllerBase
    {
        private readonly ILineService _lineService;
        private readonly ILogger<LinesController> _logger;
        public LinesController(ILineService lineService, ILogger<LinesController> logger )
        {
            _lineService = lineService;
            _logger = logger;
        }

        /// <summary>
        /// Gets all lines from the database.
        /// </summary>
        /// <returns>Return a list of LineResponseDTOs</returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<LineDto>>> GetLines()
        {
            _logger.LogInformation("GetLines endpoint called");

            var lines = await _lineService.GetLinesAsync();

            return Ok(lines);  
        }


        /// <summary>
        /// Gets a line from the database based on line Id.
        /// </summary>
        /// <param name="lineId">Line id</param>
        /// <returns>Return a LineResponseDTO</returns>
        [HttpGet("{lineId}")]
        public async Task<ActionResult<LineDto>> GetLine(string lineId)
        {
            _logger.LogInformation("GetLine endpoint called");

            LineDto? line = await _lineService.GetLineByLineIdAsync(lineId);

            return Ok(line);
        }
    }
}
