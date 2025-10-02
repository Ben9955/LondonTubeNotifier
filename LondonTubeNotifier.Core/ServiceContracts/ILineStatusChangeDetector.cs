using LondonTubeNotifier.Core.Domain.Entities;

namespace LondonTubeNotifier.Core.ServiceContracts
{
    /// <summary>
    /// Detects changes between two snapshots of line statuses.
    /// </summary>
    public interface ILineStatusChangeDetector
    {
        /// <summary>
        /// Compares the current and previous status dictionaries and returns lines that changed.
        /// </summary>
        /// <param name="currentStatus">Current line statuses.</param>
        /// <param name="previousStatus">Previous line statuses.</param>
        /// <returns>Dictionary of line IDs to their new statuses.</returns>
        Dictionary<string, List<LineStatus>> DetectChanges(
            Dictionary<string, HashSet<LineStatus>> currentStatus,
            Dictionary<string, HashSet<LineStatus>> previousStatus);
    }
}
