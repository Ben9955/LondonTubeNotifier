using LondonTubeNotifier.Core.Domain.Entities;
using LondonTubeNotifier.Core.Domain.RespositoryContracts;
using LondonTubeNotifier.Core.DTOs;
using LondonTubeNotifier.Core.MapperContracts;
using LondonTubeNotifier.Core.ServiceContracts;
using Microsoft.Extensions.Logging;

namespace LondonTubeNotifier.Core.Services
{
    public class TflStatusMonitorService : ITflStatusMonitorService
    {
        private static readonly HashSet<LineStatus> EmptyLineStatuses = new HashSet<LineStatus>();

        private readonly ITflApiService _tflApiService;
        private readonly ILineStatusRepository _lineStatusRepository;
        private readonly IUserRepository _userRepository;
        private readonly INotificationService _notificationService;
        private readonly ILogger<TflStatusMonitorService> _logger;
        private readonly ILineStatusToDtoMapper _lineStatusToDtoMapper;
        private readonly ILineStatusChangeDetector _changeDetector;
        private readonly INotificationMapper _notificationMapper;
        private readonly SemaphoreSlim _notificationSemaphore = new SemaphoreSlim(20);


        public TflStatusMonitorService(ITflApiService tflApiService, ILineStatusRepository lineStatusRepository,
            INotificationService notificationService, ILogger<TflStatusMonitorService> logger,
            IUserRepository userRepository, ILineStatusToDtoMapper lineStatusToDtoMapper,
            ILineStatusChangeDetector changeDetector, INotificationMapper notificationMapper
            )
        {
            _tflApiService = tflApiService;
            _lineStatusRepository = lineStatusRepository;
            _userRepository = userRepository;
            _notificationService = notificationService;
            _logger = logger;
            _lineStatusToDtoMapper = lineStatusToDtoMapper;
            _changeDetector = changeDetector;
            _notificationMapper = notificationMapper;
        }
        public async Task CheckForUpdatesAndNotifyAsync(CancellationToken cancellationToken)
        {
            // New status from TFL Api
            var currentStatus = await _tflApiService.GetLinesStatusAsync(cancellationToken);

            // Old status from the database
            var previousStatus = await _lineStatusRepository.GetLatestLineStatusesAsync(cancellationToken);

            var changedLines = _changeDetector.DetectChanges(currentStatus, previousStatus);

            if (changedLines.Any())
            {
                _logger.LogInformation("Detected {count} changed lines: {lineIds}", changedLines.Count, string.Join(", ", changedLines.Keys));

                // Save new status to DB
                await _lineStatusRepository.UpdateLinesAsync(changedLines, cancellationToken);

                // Broadcast the full list of lines to the homepage
                var allLineStatusesDto = changedLines
                    .Select(kv => _lineStatusToDtoMapper.Map(kv.Key, kv.Value))
                    .ToList();
                await _notificationService.NotifyAllAsync(new FullStatusNotificationDto { UpdatedLines = allLineStatusesDto }, cancellationToken);

                // Notify subscribed users
                var usersSubscriptions = await _userRepository.GetUsersByLineIdsAsync(changedLines.Keys, cancellationToken);

                _logger.LogInformation("Found {UserCount} users subscribed to changed lines", usersSubscriptions.Count);

                Dictionary<string, List<UserSubscriptionDto>> usersByLineId = usersSubscriptions
                    .GroupBy(us => us.LineId)
                    .ToDictionary(g => g.Key, g => g.ToList());

                _logger.LogInformation("Prepared user groups for {LineCount} changed lines", usersByLineId.Count);

                foreach (var changedLineGroup in changedLines)
                {
                    if (usersByLineId.TryGetValue(changedLineGroup.Key, out var recipients))
                    {

                        var lineStatusesDto = _lineStatusToDtoMapper.Map(changedLineGroup.Key, changedLineGroup.Value);

                        _logger.LogInformation("Sending notifications for {LineName} to {UserCount} users",
                            lineStatusesDto.LineName, recipients.Count);

                        var tasks = recipients.Select(async recipient =>
                        {
                            await _notificationSemaphore.WaitAsync(cancellationToken);
                            try
                            {
                                var notificationDto = _notificationMapper.Map(recipient, lineStatusesDto);
                                await _notificationService.NotifyLineSubscribersAsync(notificationDto, cancellationToken);
                            }
                            finally
                            {
                                _notificationSemaphore.Release();
                            }
                        });

                        await Task.WhenAll(tasks);


                        _logger.LogInformation("Notifications for {LineName} to {UserCount} users have been sent",
                            lineStatusesDto.LineName, recipients.Count);
                    }
                }
            }
        }

    }
}

