 using ApiShield.Api.Messaging;

public interface IUsageProcessingService
{
    Task ProcessAsync(
        UsageIncrementRequested message,
        CancellationToken cancellationToken);
}