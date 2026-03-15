namespace ApiShield.Api.Messaging;

public interface IUsageEventQueue
{
    Task EnqueueAsync(
        UsageIncrementRequested message,
        CancellationToken cancellationToken = default);
}