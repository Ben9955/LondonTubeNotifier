using LondonTubeNotifier.Core.Configuration;
using LondonTubeNotifier.Core.ServiceContracts;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;


namespace LondonTubeNotifier.Infrastructure.Workers
{
    public class TflLineStatusWorker : BackgroundService
    {
        private readonly ILogger<TflLineStatusWorker> _logger;
        private readonly ITflStatusMonitorService _monitorService;
        private readonly int _pollingIntervalSeconds;
        public TflLineStatusWorker(ILogger<TflLineStatusWorker> logger, ITflStatusMonitorService monitorService, IOptions<TflSettings> options)
        {
            _logger = logger;
            _monitorService = monitorService;
            _pollingIntervalSeconds = options.Value.PollingIntervalSeconds;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("TfL Line Status Worker is starting.");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    _logger.LogDebug("Worker running at: {time}", DateTimeOffset.Now);

                    await _monitorService.CheckForUpdatesAndNotifyAsync(stoppingToken);

                }
                catch (TaskCanceledException)
                {
                    _logger.LogInformation("TfL Line Status Worker is stopping.");
                    break;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "An unhandled exception occurred in the TfL Line Status Worker.");
                }

                await Task.Delay(_pollingIntervalSeconds * 1000, stoppingToken);

            }
        }
    }

}