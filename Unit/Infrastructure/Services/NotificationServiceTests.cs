using LondonTubeNotifier.Core.DTOs;
using LondonTubeNotifier.Core.ServiceContracts;
using LondonTubeNotifier.Core.Services;
using Microsoft.Extensions.Logging;
using Moq;

namespace UnitTests.Infrastructure.Services
{
    public class NotificationServiceTests
    {
        private readonly Mock<IUserOnlineChecker> _onlineCheckerMock;
        private readonly Mock<IEmailNotifier> _emailServiceMock;
        private readonly Mock<IRealtimeNotifier> _realtimeNotifierMock;
        private readonly Mock<ILogger<NotificationService>> _loggerMock;
        private readonly NotificationService _service;

        public NotificationServiceTests()
        {
            _onlineCheckerMock = new Mock<IUserOnlineChecker>();
            _emailServiceMock = new Mock<IEmailNotifier>();
            _realtimeNotifierMock = new Mock<IRealtimeNotifier>();
            _loggerMock = new Mock<ILogger<NotificationService>>();

            _service = new NotificationService(
                _onlineCheckerMock.Object,
                _emailServiceMock.Object,
                _realtimeNotifierMock.Object,
                _loggerMock.Object
            );
        }

        private NotificationDto CreateTestNotification(string userId = "user-1")
        {
            return new NotificationDto
            {
                RecipientId = userId,
                RecipientName = "Test User",
                RecipientEmail = "test@example.com",
                LineUpdates = new LineStatusesDto
                {
                    LineName = "Victoria",
                    StatusDescriptions = new List<StatusesDto>
                    {
                        new StatusesDto { StatusSeverity = 5, StatusDescription = "Good Service" }
                    }
                }
            };
        }

        [Fact]
        public async Task NotifyLineSubscribersAsync_ShouldCallRealtimeNotifier_WhenUserIsOnline()
        {
            // Arrange
            var notification = CreateTestNotification();
            _onlineCheckerMock.Setup(x => x.IsUserOnline(notification.RecipientId)).Returns(true);

            // Act
            await _service.NotifyLineSubscribersAsync(notification, CancellationToken.None);

            // Assert
            _realtimeNotifierMock.Verify(x => x.NotifyUserAsync(notification.RecipientId, notification, It.IsAny<CancellationToken>()), Times.Once);
            _emailServiceMock.Verify(x => x.SendEmailAsync(It.IsAny<NotificationDto>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task NotifyLineSubscribersAsync_ShouldCallEmailService_WhenUserIsOffline()
        {
            // Arrange
            var notification = CreateTestNotification();
            _onlineCheckerMock.Setup(x => x.IsUserOnline(notification.RecipientId)).Returns(false);

            // Act
            await _service.NotifyLineSubscribersAsync(notification, CancellationToken.None);

            // Assert
            _emailServiceMock.Verify(x => x.SendEmailAsync(notification, It.IsAny<CancellationToken>()), Times.Once);
            _realtimeNotifierMock.Verify(x => x.NotifyUserAsync(It.IsAny<string>(), It.IsAny<NotificationDto>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task NotifyLineSubscribersAsync_ShouldLogDebugMessages()
        {
            // Arrange
            var notification = CreateTestNotification();
            _onlineCheckerMock.Setup(x => x.IsUserOnline(notification.RecipientId)).Returns(false);

            // Act
            await _service.NotifyLineSubscribersAsync(notification, CancellationToken.None);

            // Assert
            _loggerMock.Verify(
                x => x.Log(
                    LogLevel.Debug,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString().Contains(notification.LineUpdates.LineName) &&
                                                     v.ToString().Contains(notification.RecipientName)),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.Exactly(2)
            );
        }

    }
}


