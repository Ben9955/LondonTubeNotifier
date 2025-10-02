using LondonTubeNotifier.Core.Domain.Entities;
using LondonTubeNotifier.Core.ServiceContracts;
using Microsoft.Extensions.Logging;

namespace LondonTubeNotifier.Core.Services
{
    public class LineStatusChangeDetector : ILineStatusChangeDetector
    {
        private static readonly HashSet<LineStatus> EmptyLineStatuses = new();

        private readonly ILogger<LineStatusChangeDetector> _logger;

        public LineStatusChangeDetector(ILogger<LineStatusChangeDetector> logger)
        {
            _logger = logger;
        }

        public Dictionary<string, List<LineStatus>> DetectChanges(
            Dictionary<string, HashSet<LineStatus>> currentStatus,
            Dictionary<string, HashSet<LineStatus>> previousStatus)
        {
            var changedLines = new Dictionary<string, List<LineStatus>>();

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
            _logger.LogInformation("Detected {ChangedLineCount} changed lines", changedLines.Count);
            return changedLines;
        }
    }
}
