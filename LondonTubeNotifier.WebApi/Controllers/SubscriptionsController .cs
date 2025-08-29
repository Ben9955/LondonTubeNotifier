using System.Security.Claims;
using LondonTubeNotifier.Core.DTOs;
using LondonTubeNotifier.Core.ServiceContracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LondonTubeNotifier.WebApi.Controllers
{
    /// <summary>
    /// Handles operations related to line subscription.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class SubscriptionsController : ControllerBase
    {
        private readonly IUserLineSubscriptionService _userLineSubscriptionService;
        private readonly ILogger<SubscriptionsController> _logger;
        public SubscriptionsController(IUserLineSubscriptionService userLineSubscriptionService, ILogger<SubscriptionsController> logger)
        {
            _userLineSubscriptionService = userLineSubscriptionService;
            _logger = logger;
        }

        /// <summary>
        /// Subscribes a user to a line
        /// </summary>
        /// <param name="lineId"></param>
        /// <returns></returns>
        [HttpPost("line/{lineId}")]
        [Authorize]
        public async Task<IActionResult> SubscribeToLine(string lineId)
        {
            _logger.LogInformation("Subscribe endpoint called");

            var userId = GetUserIdFromClaims();

            await _userLineSubscriptionService.SubscribeAsync(userId, lineId);

            return NoContent();

        }

        /// <summary>
        /// Delete a user subscriptin to a line
        /// </summary>
        /// <param name="lineId"></param>
        /// <returns></returns>
        [HttpDelete("line/{lineId}")]
        [Authorize]
        public async Task<IActionResult> DeleteSubscription(string lineId)
        {
            _logger.LogInformation("DeleteSubscription endpoint called");

            var userId = GetUserIdFromClaims();

            await _userLineSubscriptionService.UnsubscribeAsync(userId, lineId);

            return NoContent();
        }

        /// <summary>
        /// Gets all the lines a user is subscribed to
        /// </summary>
        /// <returns>lines</returns>
        [HttpGet]
        [Authorize]
        public async Task<ActionResult<IEnumerable<LineDto>>> GetLinesForUser()
        {
            _logger.LogInformation("GetLinesForUser endpoint called");

            var userId = GetUserIdFromClaims();

            var response = await _userLineSubscriptionService.GetUserSubscriptionsAsync(userId);

            return Ok(response);

        }

        private Guid GetUserIdFromClaims()
        {
            var userIdString = User.FindFirstValue("sub");
            if (Guid.TryParse(userIdString, out var userId))
                return userId;

            throw new UnauthorizedAccessException("Invalid user ID in token");
        }
    }
}
