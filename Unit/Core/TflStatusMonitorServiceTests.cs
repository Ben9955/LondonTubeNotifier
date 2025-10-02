using AutoFixture;
using LondonTubeNotifier.Core.Domain.Entities;
using LondonTubeNotifier.Core.Domain.RespositoryContracts;
using LondonTubeNotifier.Core.DTOs;
using LondonTubeNotifier.Core.ServiceContracts;
using LondonTubeNotifier.Core.Services;
using Microsoft.Extensions.Logging;
using Moq;

namespace UnitTests.Core
{
    public class TflStatusMonitorServiceTests
    {
        private readonly Mock<ITflApiService> _tflApiService;
        private readonly Mock<ILineStatusRepository> _lineStatusRepository;
        private readonly Mock<IUserRepository> _userRepository;
        private readonly Mock<INotificationService> _notificationService;
        private readonly Mock<ILogger<TflStatusMonitorService>> _logger;
        private readonly Fixture _fixture;
        private readonly TflStatusMonitorService _service;

        public TflStatusMonitorServiceTests()
        {
            _tflApiService = new Mock<ITflApiService>();
            _lineStatusRepository = new Mock<ILineStatusRepository>();
            _userRepository = new Mock<IUserRepository>();
            _notificationService = new Mock<INotificationService>();
            _logger = new Mock<ILogger<TflStatusMonitorService>>();
            _fixture = new Fixture();

            _service = new TflStatusMonitorService(
                _tflApiService.Object,
                _lineStatusRepository.Object,
                _notificationService.Object,
                _logger.Object,
                _userRepository.Object);
        }

        [Fact]
        public async Task CheckForUpdatesAndNotifyAsync_ShouldDoNothing_WhenNoChanges()
        {
            // Arrange
            var lineId = "central";
            var status = new HashSet<LineStatus> { new LineStatus { StatusSeverity = 0, StatusDescription = "Good Service" } };
            var current = new Dictionary<string, HashSet<LineStatus>> { { lineId, status } };
            var previous = new Dictionary<string, HashSet<LineStatus>> { { lineId, status } };

            _tflApiService.Setup(s => s.GetLinesStatusAsync(CancellationToken.None)).ReturnsAsync(current);
            _lineStatusRepository.Setup(r => r.GetLatestLineStatusesAsync(CancellationToken.None)).ReturnsAsync(previous);

            // Act
            await _service.CheckForUpdatesAndNotifyAsync(CancellationToken.None);

            // Assert
            _lineStatusRepository.Verify(r => r.UpdateLinesAsync(It.IsAny<Dictionary<string, List<LineStatus>>>(), It.IsAny<CancellationToken>()), Times.Never);
            _notificationService.Verify(n => n.NotifyLineSubscribersAsync(It.IsAny<NotificationDto>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task CheckForUpdatesAndNotifyAsync_ShouldSaveAndNotify_WhenStatusChanged()
        {
            // Arrange
            var lineId = "central";
            var oldStatus = new HashSet<LineStatus> { new LineStatus { StatusSeverity = 0, StatusDescription = "Good Service" } };
            var newStatus = new HashSet<LineStatus> { new LineStatus { StatusSeverity = 10, StatusDescription = "Severe Delays" } };
            var current = new Dictionary<string, HashSet<LineStatus>> { { lineId, newStatus } };
            var previous = new Dictionary<string, HashSet<LineStatus>> { { lineId, oldStatus } };
            var changedStatus = new List<LineStatus> { new LineStatus { StatusSeverity = 10, StatusDescription = "Severe Delays" } };
            var update = new Dictionary<string, List<LineStatus>> { { lineId, changedStatus } };


            var user = _fixture.Build<UserSubscriptionDto>()
                .With(u => u.LineId, lineId)
                .Create();

            _tflApiService.Setup(s => s.GetLinesStatusAsync(CancellationToken.None)).ReturnsAsync(current);
            _lineStatusRepository.Setup(r => r.GetLatestLineStatusesAsync(CancellationToken.None)).ReturnsAsync(previous);
            _userRepository.Setup(r => r.GetUsersByLineIdsAsync(It.IsAny<IEnumerable<string>>(), CancellationToken.None))
                .ReturnsAsync(new List<UserSubscriptionDto> { user });

            // Act
            await _service.CheckForUpdatesAndNotifyAsync(CancellationToken.None);

            // Assert
            _lineStatusRepository.Verify(r => r.UpdateLinesAsync(update, CancellationToken.None), Times.Once);
            _notificationService.Verify(n => n.NotifyLineSubscribersAsync(It.Is<NotificationDto>(dto =>
                dto.RecipientId == user.UserId &&
                dto.LineUpdates.LineId == lineId
            ), CancellationToken.None), Times.Once);
        }

        [Fact]
        public async Task CheckForUpdatesAndNotifyAsync_ShouldSaveButNotNotify_WhenNoUsersSubscribed()
        {
            // Arrange
            var lineId = "central";
            var oldStatus = new HashSet<LineStatus> { new LineStatus { StatusSeverity = 0, StatusDescription = "Good Service" } };
            var newStatus = new HashSet<LineStatus> { new LineStatus { StatusSeverity = 6, StatusDescription = "Minor Delays" } };
            var current = new Dictionary<string, HashSet<LineStatus>> { { lineId, newStatus } };
            var previous = new Dictionary<string, HashSet<LineStatus>> { { lineId, oldStatus } };
            var changedStatus = new List<LineStatus> { new LineStatus { StatusSeverity = 6, StatusDescription = "Minor Delays" } };
            var update = new Dictionary<string, List<LineStatus>> { { lineId, changedStatus } };



            _tflApiService.Setup(s => s.GetLinesStatusAsync(CancellationToken.None)).ReturnsAsync(current);
            _lineStatusRepository.Setup(r => r.GetLatestLineStatusesAsync(CancellationToken.None)).ReturnsAsync(previous);
            _userRepository.Setup(r => r.GetUsersByLineIdsAsync(It.IsAny<IEnumerable<string>>(), CancellationToken.None))
                .ReturnsAsync(new List<UserSubscriptionDto>()); // no users

            // Act
            await _service.CheckForUpdatesAndNotifyAsync(CancellationToken.None);

            // Assert
            _lineStatusRepository.Verify(r => r.UpdateLinesAsync(update, CancellationToken.None), Times.Once);
            _notificationService.Verify(n => n.NotifyLineSubscribersAsync(It.IsAny<NotificationDto>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task CheckForUpdatesAndNotifyAsync_ShouldNotifyEachUser_WhenMultipleSubscribed()
        {
            // Arrange
            var lineId = "central";
            var oldStatus = new HashSet<LineStatus> { new LineStatus { StatusSeverity = 0, StatusDescription = "Good Service" } };
            var newStatus = new HashSet<LineStatus> { new LineStatus { StatusSeverity = 10, StatusDescription = "Severe Delays" } };
            var current = new Dictionary<string, HashSet<LineStatus>> { { lineId, newStatus } };
            var previous = new Dictionary<string, HashSet<LineStatus>> { { lineId, oldStatus } };
            var changedStatus = new List<LineStatus> { new LineStatus { StatusSeverity = 10, StatusDescription = "Severe Delays" } };
            var update = new Dictionary<string, List<LineStatus>> { { lineId, changedStatus } };


            var users = _fixture.CreateMany<UserSubscriptionDto>(3)
                .Select(u => { u.LineId = lineId; return u; })
                .ToList();

            _tflApiService.Setup(s => s.GetLinesStatusAsync(CancellationToken.None)).ReturnsAsync(current);
            _lineStatusRepository.Setup(r => r.GetLatestLineStatusesAsync(CancellationToken.None)).ReturnsAsync(previous);
            _userRepository.Setup(r => r.GetUsersByLineIdsAsync(It.IsAny<IEnumerable<string>>(), CancellationToken.None))
                .ReturnsAsync(users);

            // Act
            await _service.CheckForUpdatesAndNotifyAsync(CancellationToken.None);

            // Assert
            _lineStatusRepository.Verify(r => r.UpdateLinesAsync(update, CancellationToken.None), Times.Once);
            _notificationService.Verify(n => n.NotifyLineSubscribersAsync(It.IsAny<NotificationDto>(), CancellationToken.None), Times.Exactly(users.Count));
        }
    }

}

