using Azure;
using LondonTubeNotifier.Core.Domain.Entities;
using LondonTubeNotifier.Core.DTOs;
using LondonTubeNotifier.Core.ServiceContracts;
using Microsoft.AspNetCore.Mvc;

namespace LondonTubeNotifier.WebApi.Controllers
{
    /// <summary>
    /// Provides endpoints to manage and retrieve information about London Tube lines.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class LinesController : ControllerBase
    {
        private readonly ILineService _lineService;
        private readonly ILogger<LinesController> _logger;

        public LinesController(ILineService lineService, ILogger<LinesController> logger)
        {
            _lineService = lineService;
            _logger = logger;
        }

        /// <summary>
        /// Retrieves all London Tube lines.
        /// </summary>
        /// <returns>A list of Tube lines.</returns>
        /// <response code="200">Returns all Tube lines.</response>
        /// <remarks>Use this to get a full list of lines available in the system.</remarks>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<LineDto>>> GetLines()
        {
            _logger.LogInformation("GetLines endpoint called");
            var lines = await _lineService.GetLinesAsync();
            return Ok(lines);
        }

        /// <summary>
        /// Retrieves a specific Tube line by its unique ID.
        /// </summary>
        /// <param name="lineId">The unique ID of the Tube line.</param>
        /// <returns>The requested Tube line.</returns>
        /// <response code="200">Returns the Tube line.</response>
        /// <response code="404">Line not found.</response>
        /// <response code="400">Line ID is empty or whitespace.</response>
        /// <remarks>Use this to get details for a single line.</remarks>
        [HttpGet("{lineId}")]
        public async Task<ActionResult<LineDto>> GetLine(string lineId)
        {
            _logger.LogInformation("GetLine endpoint called");
            var line = await _lineService.GetLineByLineIdAsync(lineId);
            return Ok(line);
        }
    }
}
