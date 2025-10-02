namespace LondonTubeNotifier.Core.ServiceContracts
{
    /// <summary>
    /// Checks for line status updates and triggers notifications if there are changes.
    /// </summary>
    public interface ITflStatusMonitorService
    {
        /// <summary>
        /// Compares the latest line statuses with what’s stored and notifies users about changes.
        /// </summary>
        /// <param name="cancellationToken">Token to cancel the operation if needed.</param>
        Task CheckForUpdatesAndNotifyAsync(CancellationToken cancellationToken);
    }
}
