using LondonTubeNotifier.Core.Configuration;
using LondonTubeNotifier.Core.ServiceContracts;
using LondonTubeNotifier.Infrastructure.Workers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;

namespace UnitTests.Infrastructure.Workers
{
    public class TflLineStatusWorkerTests
    {
        private readonly Mock<ILogger<TflLineStatusWorker>> _loggerMock;
        private readonly Mock<IServiceScopeFactory> _scopeFactoryMock;
        private readonly Mock<IServiceScope> _scopeMock;
        private readonly Mock<IServiceProvider> _serviceProviderMock;
        private readonly Mock<ITflStatusMonitorService> _monitorServiceMock;

        public TflLineStatusWorkerTests()
        {
            _loggerMock = new Mock<ILogger<TflLineStatusWorker>>();
            _scopeFactoryMock = new Mock<IServiceScopeFactory>();
            _scopeMock = new Mock<IServiceScope>();
            _serviceProviderMock = new Mock<IServiceProvider>();
            _monitorServiceMock = new Mock<ITflStatusMonitorService>();

            _scopeMock.Setup(s => s.ServiceProvider).Returns(_serviceProviderMock.Object);
            _scopeFactoryMock.Setup(f => f.CreateScope()).Returns(_scopeMock.Object);
            _serviceProviderMock
                .Setup(p => p.GetService(typeof(ITflStatusMonitorService)))
                .Returns(_monitorServiceMock.Object);
        }

        [Fact]
        public async Task ExecuteAsync_ShouldCallMonitorService_AndLog()
        {
            // Arrange
            var options = Options.Create(new TflSettings { PollingIntervalSeconds = 1 });
            var worker = new TflLineStatusWorker(
                _loggerMock.Object,
                _scopeFactoryMock.Object,
                options);

            using var cts = new CancellationTokenSource();
            cts.CancelAfter(100); // cancel quickly after 1 iteration

            // Act
            await worker.StartAsync(cts.Token);

            // Assert
            _monitorServiceMock.Verify(
                m => m.CheckForUpdatesAndNotifyAsync(It.IsAny<CancellationToken>()),
                Times.AtLeastOnce);

            _loggerMock.Verify(
                l => l.Log(
                    LogLevel.Information,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("starting")),
                    null,
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Once);
        }
    }
}
