using LondonTubeNotifier.Core.Configuration;
using LondonTubeNotifier.Core.DTOs.TflDtos;
using LondonTubeNotifier.Core.ServiceContracts;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;


namespace LondonTubeNotifier.Infrastructure.Workers
{
    public class TflLineStatusWorker : BackgroundService
    {
        private readonly ILogger<TflLineStatusWorker> _logger;
        private readonly ITflApiService _tflApi;
        private readonly TflSettings _settings;
        public TflLineStatusWorker(ILogger<TflLineStatusWorker> logger, ITflApiService tflApiService, IOptions<TflSettings> options)
        {
            _logger = logger;
            _tflApi = tflApiService;
            _settings = options.Value;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                if (_logger.IsEnabled(LogLevel.Information))
                {
                    _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                }

                 List<LineStatusDto>? linesStatus = await _tflApi.GetLinesStatusAsync();

                linesStatus?.ForEach(l =>
                {
                    Console.WriteLine(l.Id);
                    l.LineStatuses.ForEach(s =>
                    {

                        Console.WriteLine(
                        $"{s.StatusSeverity} {s.StatusSeverityDescription}"
                            );
                    });
                });

                await Task.Delay(_settings.PollingIntervalSeconds * 1000, stoppingToken);
            }
        }
    }

}