using AutoFixture;
using FluentAssertions;
using LondonTubeNotifier.Core.Domain.Entities;
using LondonTubeNotifier.Core.Domain.RespositoryContracts;
using LondonTubeNotifier.Core.Exceptions;
using LondonTubeNotifier.Core.Services;
using LondonTubeNotifier.Infrastructure.Entities;
using Moq;

namespace UnitTests
{
    public class UserLineSubscriptionServiceTest
    {
        private readonly Mock<ILineRepository> _lineRepository;
        private readonly Mock<IUserRepository> _userRepository;
        private readonly Fixture _fixture;
        private readonly UserLineSubscriptionService _service;

        public UserLineSubscriptionServiceTest()
        {
            _lineRepository = new Mock<ILineRepository>();
            _userRepository = new Mock<IUserRepository>();
            _service = new UserLineSubscriptionService(_lineRepository.Object, _userRepository.Object);
            _fixture = new Fixture();
            _fixture.Behaviors
                .OfType<ThrowingRecursionBehavior>()
                .ToList()
                .ForEach(b => _fixture.Behaviors.Remove(b));
            _fixture.Behaviors.Add(new OmitOnRecursionBehavior());
        }



        #region Subscribe
        [Fact]
        public async Task SubscribeAsync_ShouldAddSubscription_WhenUserAndLineExist_AndNotSubscribed()
        {
            // Arrange
            var user = _fixture.Build<ApplicationUser>().Without(u => u.Subscriptions).Create();
            var line = _fixture.Create<Line>();

            _userRepository.Setup(r => r.GetUserWithSubscriptionsAsync(user.Id, CancellationToken.None))
                .ReturnsAsync(user);

            _lineRepository.Setup(r => r.GetLineByLineIdAsync(line.Id))
                .ReturnsAsync(line);

            // Act
            await _service.SubscribeAsync(user.Id, line.Id);

            // Assert
            _lineRepository.Verify(r => r.AddSubscriptionAsync(user, line), Times.Once);
        }

        [Fact]
        public async Task SubscribeAsync_ShouldThrow_WhenUserNotFound()
        {
            var userId = Guid.NewGuid();
            _userRepository.Setup(r => r.GetUserWithSubscriptionsAsync(userId, CancellationToken.None))
                .ReturnsAsync((ApplicationUser?)null);

            await FluentActions.Invoking(() => _service.SubscribeAsync(userId, "lineId"))
                .Should().ThrowAsync<DomainNotFoundException>();
        }

        [Fact]
        public async Task SubscribeAsync_ShouldThrow_WhenLineNotFound()
        {
            var user = _fixture.Build<ApplicationUser>().Without(u => u.Subscriptions).Create();
            _userRepository.Setup(r => r.GetUserWithSubscriptionsAsync(user.Id, CancellationToken.None))
                .ReturnsAsync(user);
            _lineRepository.Setup(r => r.GetLineByLineIdAsync(It.IsAny<string>()))
                .ReturnsAsync((Line?)null);

            await FluentActions.Invoking(() => _service.SubscribeAsync(user.Id, "lineId"))
                .Should().ThrowAsync<DomainNotFoundException>();
        }


        [Fact]
        public async Task SubscribeAsync_ShouldThrow_WhenAlreadySubscribed()
        {
            var line = _fixture.Create<Line>();
            var user = _fixture.Build<ApplicationUser>()
                .With(u => u.Subscriptions, new List<Line> { line })
                .Create();

            _userRepository.Setup(r => r.GetUserWithSubscriptionsAsync(user.Id, CancellationToken.None))
                .ReturnsAsync(user);

            _lineRepository.Setup(r => r.GetLineByLineIdAsync(line.Id))
                .ReturnsAsync(line);

            await FluentActions.Invoking(() => _service.SubscribeAsync(user.Id, line.Id))
                .Should().ThrowAsync<DomainValidationException>();
        }

        #endregion


        #region Unsubscribe
        [Fact]
        public async Task UnsubscribeAsync_ShouldRemoveSubscription_WhenSubscribed()
        {
            var line = _fixture.Create<Line>();
            var user = _fixture.Build<ApplicationUser>()
                .With(u => u.Subscriptions, new List<Line> { line })
                .Create();

            _userRepository.Setup(r => r.GetUserWithSubscriptionsAsync(user.Id, CancellationToken.None))
                .ReturnsAsync(user);
            _lineRepository.Setup(r => r.GetLineByLineIdAsync(line.Id))
                .ReturnsAsync(line);

            await _service.UnsubscribeAsync(user.Id, line.Id);

            _lineRepository.Verify(r => r.DeleteSubscriptionAsync(user, line), Times.Once);
        }

        [Fact]
        public async Task UnsubscribeAsync_ShouldThrow_WhenUserNotFound()
        {
            var userId = Guid.NewGuid();
            _userRepository.Setup(r => r.GetUserWithSubscriptionsAsync(userId, CancellationToken.None))
                .ReturnsAsync((ApplicationUser?)null);

            await FluentActions.Invoking(() => _service.SubscribeAsync(userId, "lineId"))
                .Should().ThrowAsync<DomainNotFoundException>();
        }

        [Fact]
        public async Task UnsubscribeAsync_ShouldThrow_WhenLineNotFound()
        {
            var user = _fixture.Build<ApplicationUser>().Without(u => u.Subscriptions).Create();
            _userRepository.Setup(r => r.GetUserWithSubscriptionsAsync(user.Id, CancellationToken.None))
                .ReturnsAsync(user);
            _lineRepository.Setup(r => r.GetLineByLineIdAsync(It.IsAny<string>()))
                .ReturnsAsync((Line?)null);

            await FluentActions.Invoking(() => _service.SubscribeAsync(user.Id, "lineId"))
                .Should().ThrowAsync<DomainNotFoundException>();
        }

        [Fact]
        public async Task UnsubscribeAsync_ShouldThrow_WhenUserNotSubscribed()
        {
            var line = _fixture.Create<Line>();
            var user = _fixture.Build<ApplicationUser>()
                .With(u => u.Subscriptions, new List<Line>())
                .Create();

            _userRepository.Setup(r => r.GetUserWithSubscriptionsAsync(user.Id, CancellationToken.None))
                .ReturnsAsync(user);
            _lineRepository.Setup(r => r.GetLineByLineIdAsync(line.Id))
                .ReturnsAsync(line);

            await FluentActions.Invoking(() => _service.UnsubscribeAsync(user.Id, line.Id))
                .Should().ThrowAsync<DomainValidationException>();
        }


        #endregion


        #region GetUserSubscriptions
        [Fact]
        public async Task GetUserSubscriptionsAsync_ShouldReturnLineDtos()
        {
            var lines = _fixture.CreateMany<Line>(3).ToList();
            var user = _fixture.Build<ApplicationUser>()
                .With(u => u.Subscriptions, lines)
                .Create();

            _userRepository.Setup(r => r.GetUserWithSubscriptionsAsync(user.Id, CancellationToken.None))
                .ReturnsAsync(user);

            var result = await _service.GetUserSubscriptionsAsync(user.Id, CancellationToken.None);

            result.Should().HaveCount(3);
            result.Select(l => l.Id).Should().BeEquivalentTo(lines.Select(l => l.Id));
        }

        [Fact]
        public async Task GetUserSubscriptionsAsync_ShouldThrow_WhenUserNotFound()
        {
            var userId = Guid.NewGuid();
            _userRepository.Setup(r => r.GetUserWithSubscriptionsAsync(userId, CancellationToken.None))
                .ReturnsAsync((ApplicationUser?)null);

            await FluentActions.Invoking(() => _service.GetUserSubscriptionsAsync(userId, CancellationToken.None))
                .Should().ThrowAsync<DomainNotFoundException>();
        }

        [Fact]
        public async Task GetUserSubscriptionsAsync_ShouldReturnEmpty_WhenNoSubscriptions()
        {
            var user = _fixture.Build<ApplicationUser>()
                .With(u => u.Subscriptions, new List<Line>())
                .Create();

            _userRepository.Setup(r => r.GetUserWithSubscriptionsAsync(user.Id, CancellationToken.None))
                .ReturnsAsync(user);

            var result = await _service.GetUserSubscriptionsAsync(user.Id, CancellationToken.None);
            result.Should().BeEmpty();
        }


        #endregion
    }
}



