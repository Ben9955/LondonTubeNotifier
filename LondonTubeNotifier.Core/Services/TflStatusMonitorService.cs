using LondonTubeNotifier.Core.Domain.Entities;
using LondonTubeNotifier.Core.Domain.RespositoryContracts;
using LondonTubeNotifier.Core.ServiceContracts;

namespace LondonTubeNotifier.Core.Services
{
    public class TflStatusMonitorService : ITflStatusMonitorService
    {
        private static readonly HashSet<LineStatus> EmptyLineStatuses = new HashSet<LineStatus>();

        private readonly ITflApiService _tflApiService;
        private readonly ILineStatusRepository _lineStatusRepository;
        public TflStatusMonitorService(ITflApiService tflApiService, ILineStatusRepository lineStatusRepository)
        {
            _tflApiService = tflApiService;
            _lineStatusRepository = lineStatusRepository;
        }
        public async Task CheckForUpdatesAndNotifyAsync(CancellationToken cancellationToken)
        {
            // New status from TFL Api
            var currentStatus = await _tflApiService.GetLinesStatusAsync(cancellationToken);

            // Old status from the database
            var previousStatus = await _lineStatusRepository.GetLatestLineStatusesAsync();

            if (DataHasChanged(currentStatus, previousStatus, out var changedLines))
            {
                // Save new to DB
                await _lineStatusRepository.SaveStatusAsync(currentStatus);


            }
        }

        private bool DataHasChanged(
            Dictionary<string, HashSet<LineStatus>> currentStatus,
            Dictionary<string, HashSet<LineStatus>> previousStatus,
            out Dictionary<string, (HashSet<LineStatus> OldStatus, HashSet<LineStatus> NewStatus)> changedLines)
        {
            changedLines = new Dictionary<string, (HashSet<LineStatus> OldStatus, HashSet<LineStatus> NewStatus)>();

            var allLineIds = currentStatus.Keys.Union(previousStatus.Keys);

            foreach (var lineId in allLineIds)
            {
                currentStatus.TryGetValue(lineId, out var newLineStatuses);
                previousStatus.TryGetValue(lineId, out var oldLineStatuses);

                var oldLStatuses = oldLineStatuses ?? EmptyLineStatuses;
                var newLStatuses = newLineStatuses ?? EmptyLineStatuses;

                if (!newLStatuses.SetEquals(oldLStatuses))
                {
                    changedLines.Add(lineId, (oldLStatuses, newLStatuses));
                }

            }
            return changedLines.Count > 0;
        }

    }
}

