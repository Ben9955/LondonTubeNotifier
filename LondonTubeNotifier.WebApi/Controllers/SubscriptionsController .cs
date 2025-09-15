using System.Security.Claims;
using LondonTubeNotifier.Core.DTOs;
using LondonTubeNotifier.Core.ServiceContracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LondonTubeNotifier.WebApi.Controllers
{
    /// <summary>
    /// Manage user subscriptions to Tube lines.
    /// Users can subscribe, unsubscribe, and view their subscribed lines.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class SubscriptionsController : ControllerBase
    {
        private readonly IUserLineSubscriptionService _subscriptionService;
        private readonly ILogger<SubscriptionsController> _logger;

        public SubscriptionsController(IUserLineSubscriptionService subscriptionService, ILogger<SubscriptionsController> logger)
        {
            _subscriptionService = subscriptionService;
            _logger = logger;
        }

        /// <summary>
        /// Subscribe the logged-in user to a Tube line.
        /// </summary>
        /// <param name="lineId">Tube line unique ID.</param>
        /// <returns>No content if successful.</returns>
        /// <response code="204">Subscription created.</response>
        /// <response code="400">Already subscribed.</response>
        /// <response code="401">User not authenticated.</response>
        /// <response code="404">Line not found.</response>
        /// <remarks>Use this to start receiving notifications for a line.</remarks>
        [HttpPost("line/{lineId}")]
        [Authorize]
        public async Task<IActionResult> SubscribeToLine(string lineId)
        {
            _logger.LogInformation("Subscribe endpoint called");
            var userId = GetUserIdFromClaims();
            await _subscriptionService.SubscribeAsync(userId, lineId);
            return NoContent();
        }

        /// <summary>
        /// Unsubscribe the logged-in user from a Tube line.
        /// </summary>
        /// <param name="lineId">Tube line unique ID.</param>
        /// <returns>No content if successful.</returns>
        /// <response code="204">Subscription removed.</response>
        /// <response code="401">User not authenticated.</response>
        /// <response code="404">Line not found or not subscribed.</response>
        /// <remarks>Use this to stop receiving notifications for a line.</remarks>
        [HttpDelete("line/{lineId}")]
        [Authorize]
        public async Task<IActionResult> DeleteSubscription(string lineId)
        {
            _logger.LogInformation("DeleteSubscription endpoint called");
            var userId = GetUserIdFromClaims();
            await _subscriptionService.UnsubscribeAsync(userId, lineId);
            return NoContent();
        }

        /// <summary>
        /// Get all Tube lines the logged-in user is subscribed to.
        /// </summary>
        /// <returns>List of subscribed Tube lines.</returns>
        /// <response code="200">Returns subscribed lines.</response>
        /// <response code="401">User not authenticated.</response>
        /// <remarks>Shows current subscriptions for the logged-in user.</remarks>
        [HttpGet]
        [Authorize]
        public async Task<ActionResult<IEnumerable<LineDto>>> GetLinesForUser()
        {
            _logger.LogInformation("GetLinesForUser endpoint called");
            var userId = GetUserIdFromClaims();
            var lines = await _subscriptionService.GetUserSubscriptionsAsync(userId, CancellationToken.None);
            return Ok(lines);
        }

        private Guid GetUserIdFromClaims()
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (Guid.TryParse(userIdString, out var userId)) return userId;
            throw new UnauthorizedAccessException("Invalid user ID in token");
        }
    }
}
