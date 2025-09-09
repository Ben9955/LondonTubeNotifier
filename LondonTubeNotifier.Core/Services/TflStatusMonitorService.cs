using LondonTubeNotifier.Core.Domain.Entities;
using LondonTubeNotifier.Core.Domain.RespositoryContracts;
using LondonTubeNotifier.Core.DTOs;
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
        public TflStatusMonitorService(ITflApiService tflApiService, ILineStatusRepository lineStatusRepository,
            INotificationService notificationService, ILogger<TflStatusMonitorService> logger,
            IUserRepository userRepository)
        {
            _tflApiService = tflApiService;
            _lineStatusRepository = lineStatusRepository;
            _userRepository = userRepository;
            _notificationService = notificationService;
            _logger = logger;
        }
        public async Task CheckForUpdatesAndNotifyAsync(CancellationToken cancellationToken)
        {
            // New status from TFL Api
            var currentStatus = await _tflApiService.GetLinesStatusAsync(cancellationToken);

            // Old status from the database
            var previousStatus = await _lineStatusRepository.GetLatestLineStatusesAsync(cancellationToken);

            if (DetectChangedLineStatuses(currentStatus, previousStatus, out var changedLines))
            {

                _logger.LogInformation("Detected {count} changed lines: {lineIds}", changedLines.Count, string.Join(", ", changedLines.Keys));

                // Save new status to DB
                await _lineStatusRepository.SaveStatusAsync(currentStatus, cancellationToken);

                var usersSubscriptions = await _userRepository.GetUsersByLineIdsAsync(changedLines.Keys, cancellationToken);

                Dictionary<string, List<UserSubscriptionDto>> usersByLineId = usersSubscriptions
                    .GroupBy(us => us.LineId)
                    .ToDictionary(g => g.Key, g => g.ToList());

                foreach (var lineStatuses in changedLines)
                {
                    if (usersByLineId.TryGetValue(lineStatuses.Key, out var recipients))
                    {
                        List<StatusesDto> statusesDto = lineStatuses.Value.Select(ls =>
                        new StatusesDto
                        {
                            StatusDescription = ls.StatusDescription,
                            Reason = ls.Reason,
                            StatusSeverity = ls.StatusSeverity,
                            StatusCssClass = GetCssClassForStatus(ls.StatusSeverity)
                        }).ToList();

                        var lineStatusesDto = new LineStatusesDto
                        {
                            LineName = Capitalize(lineStatuses.Key),
                            LineId = lineStatuses.Key,
                            StatusDescriptions = statusesDto
                        };
                        
                        _logger.LogInformation("Sending notifications for {LineName} to {UserCount} users",
                            lineStatusesDto.LineName, recipients.Count);

                        foreach (var recipient in recipients)
                        {
                            NotificationDto notificationDto = new NotificationDto
                            {
                                RecipientEmail = recipient.Email,
                                RecipientName = recipient.FullName,
                                RecipientId = recipient.UserId,
                                LineUpdates = lineStatusesDto
                            };

                            // If the user is online will send a notification via webscoket otherwise email
                            await _notificationService.NotifyLineSubscribersAsync(notificationDto, cancellationToken);
                        }

                        _logger.LogInformation("Notifications for {LineName} to {UserCount} users have been sent",
                            lineStatusesDto.LineName, recipients.Count);
                    }
                }
            }
        }

        private bool DetectChangedLineStatuses(
            Dictionary<string, HashSet<LineStatus>> currentStatus,
            Dictionary<string, HashSet<LineStatus>> previousStatus,
            out Dictionary<string, List<LineStatus>> changedLines)
        {
            changedLines = new Dictionary<string, List<LineStatus>>();

            var allLineIds = currentStatus.Keys.Union(previousStatus.Keys);

            foreach (var lineId in allLineIds)
            {
                currentStatus.TryGetValue(lineId, out var newLineStatuses);
                previousStatus.TryGetValue(lineId, out var oldLineStatuses);

                var oldLStatuses = oldLineStatuses ?? EmptyLineStatuses;
                var newLStatuses = newLineStatuses ?? EmptyLineStatuses;

                var newStatuses = newLStatuses.Except(oldLStatuses);
                if (newStatuses.Any())
                {
                    changedLines.Add(lineId, newStatuses.ToList());
                }
            }
            return changedLines.Count > 0;
        }

        private string Capitalize(string input)
        {
            if (string.IsNullOrEmpty(input)) return input;
            return char.ToUpper(input[0]) + input.Substring(1);
        }


        private string GetCssClassForStatus(int severity)
        {
            // Switch to map severity to a CSS class
            return severity switch
            {
                >= 10 => "severe-delays",
                >= 6 => "minor-delays",
                _ => "good-service",
            };
        }

    }
}

