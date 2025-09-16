using System.Security.Claims;
using FluentAssertions;
using LondonTubeNotifier.Core.DTOs;
using LondonTubeNotifier.Core.ServiceContracts;
using LondonTubeNotifier.WebApi.Controllers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;

namespace UnitTests.WebApi
{
    public class SubscriptionsControllerTests
    {
        private readonly Mock<IUserLineSubscriptionService> _subscriptionService;
        private readonly Mock<ILogger<SubscriptionsController>> _logger;
        private readonly SubscriptionsController _controller;

        public SubscriptionsControllerTests()
        {
            _subscriptionService = new Mock<IUserLineSubscriptionService>();
            _logger = new Mock<ILogger<SubscriptionsController>>();
            _controller = new SubscriptionsController(_subscriptionService.Object, _logger.Object);

            // Fake authenticated user
            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.NameIdentifier, Guid.NewGuid().ToString())
            }, "mock"));

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = user }
            };
        }

        [Fact]
        public async Task SubscribeToLine_ShouldCallServiceAndReturnNoContent()
        {
            var lineId = "victoria";

            var result = await _controller.SubscribeToLine(lineId);

            result.Should().BeOfType<NoContentResult>();
            _subscriptionService.Verify(s => s.SubscribeAsync(It.IsAny<Guid>(), lineId), Times.Once);
        }

        [Fact]
        public async Task DeleteSubscription_ShouldCallServiceAndReturnNoContent()
        {
            var lineId = "central";

            var result = await _controller.DeleteSubscription(lineId);

            result.Should().BeOfType<NoContentResult>();
            _subscriptionService.Verify(s => s.UnsubscribeAsync(It.IsAny<Guid>(), lineId), Times.Once);
        }

        [Fact]
        public async Task GetLinesForUser_ShouldReturnOkWithSubscriptions()
        {
            var subscriptions = new List<LineDto>
            {
                new LineDto { Id = "victoria", Name = "Victoria Line" },
                new LineDto { Id = "central", Name = "Central Line" }
            };
            _subscriptionService.Setup(s => s.GetUserSubscriptionsAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(subscriptions);

            var result = await _controller.GetLinesForUser();

            result.Result.Should().BeOfType<OkObjectResult>()
                .Which.Value.Should().BeAssignableTo<IEnumerable<LineDto>>()
                .Which.Should().BeEquivalentTo(subscriptions);
        }
    }
}
