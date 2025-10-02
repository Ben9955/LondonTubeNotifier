using LondonTubeNotifier.Core.Configuration;
using LondonTubeNotifier.Core.ServiceContracts;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;


namespace LondonTubeNotifier.Infrastructure.Workers
{
    public class TflLineStatusWorker : BackgroundService
    {
        private readonly ILogger<TflLineStatusWorker> _logger;
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly int _pollingIntervalSeconds;
        public TflLineStatusWorker(
            ILogger<TflLineStatusWorker> logger, 
            IServiceScopeFactory scopeFactory,
            IOptions<TflSettings> options)
        {
            _logger = logger;
            _scopeFactory = scopeFactory;
            _pollingIntervalSeconds = options.Value.PollingIntervalSeconds;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var backoffSeconds = _pollingIntervalSeconds;

            _logger.LogInformation("TfL Line Status Worker is starting.");

            while (!stoppingToken.IsCancellationRequested)
            {
                using var scope = _scopeFactory.CreateScope();
                var monitorService = scope.ServiceProvider.GetRequiredService<ITflStatusMonitorService>();

                try
                {
                    _logger.LogDebug("Worker running at: {time}", DateTimeOffset.Now);

                    await monitorService.CheckForUpdatesAndNotifyAsync(stoppingToken);

                    backoffSeconds = _pollingIntervalSeconds;
                }
                catch (TaskCanceledException)
                {
                    _logger.LogInformation("TfL Line Status Worker is stopping.");
                    break;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "An unhandled exception occurred in the TfL Line Status Worker.");

                    backoffSeconds = Math.Min(backoffSeconds * 2, 3600);
                }

                await Task.Delay(backoffSeconds * 1000, stoppingToken);
                _logger.LogDebug("Next check in {Seconds} seconds", backoffSeconds);

            }
        }
    }

}