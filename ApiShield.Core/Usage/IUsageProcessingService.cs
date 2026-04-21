namespace ApiShield.Core.Usage;

public interface IUsageProcessingService
{
    Task ProcessAsync(
        UsageIncrementRequested message,
        CancellationToken cancellationToken);
}