namespace LondonTubeNotifier.WorkerService
{
    public class TflLineStatusWorker : BackgroundService
    {
        private readonly ILogger<TflLineStatusWorker> _logger;

        public TflLineStatusWorker(ILogger<TflLineStatusWorker> logger)
        {
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                if (_logger.IsEnabled(LogLevel.Information))
                {
                    _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                }
                await Task.Delay(1000, stoppingToken);
            }
        }
    }
}
