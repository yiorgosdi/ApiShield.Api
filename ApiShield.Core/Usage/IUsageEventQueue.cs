namespace ApiShield.Core.Usage;

public interface IUsageEventQueue
{
    Task EnqueueAsync(
        UsageIncrementRequested message,
        CancellationToken cancellationToken = default);
}