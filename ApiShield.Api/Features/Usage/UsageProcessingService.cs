using ApiShield.Api.Messaging;
using ApiShield.Core.Usage;

public sealed class UsageProcessingService : IUsageProcessingService
{
    private readonly IApiKeyUsageService _usageService;
    private readonly ILogger<UsageProcessingService> _logger;

    public UsageProcessingService(
        IApiKeyUsageService usageService,
        ILogger<UsageProcessingService> logger)
    {
        _usageService = usageService;
        _logger = logger;
    }

    public async Task ProcessAsync(
        UsageIncrementRequested message,
        CancellationToken cancellationToken)
    {
        var date = DateOnly.FromDateTime(message.OccurredAtUtc);

        var result = await _usageService.IncrementAsync(
            message.ApiKey,
            date,
            cancellationToken);

        _logger.LogInformation(
            "Processed usage event {EventId} ApiKey={ApiKey} Route={Route} NewCount={NewCount}",
            message.EventId,
            message.ApiKey,
            message.Route,
            result.NewCount);
    }
}