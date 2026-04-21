using ApiShield.Core.Idempotency;
using Microsoft.Extensions.Logging;

namespace ApiShield.Core.Usage;

public sealed class UsageProcessingService : IUsageProcessingService
{
    private readonly IApiKeyUsageService _usageService;
    private readonly IIdempotencyService _idempotencyService;
    private readonly ILogger<UsageProcessingService> _logger;

    public UsageProcessingService(
        IApiKeyUsageService usageService,
        IIdempotencyService idempotencyService,
        ILogger<UsageProcessingService> logger)
    {
        _usageService = usageService;
        _idempotencyService = idempotencyService;
        _logger = logger;
    }

    public async Task ProcessAsync(UsageIncrementRequested message, CancellationToken ct)
    {
        if (await _idempotencyService.IsAlreadyProcessedAsync(
                message.ApiKeyId,
                message.IdempotencyKey,
                ct))
        {
            _logger.LogInformation(
                "Skipping duplicate processed usage event. ApiKeyId: {ApiKeyId}, IdempotencyKey: {IdempotencyKey}",
                message.ApiKeyId,
                message.IdempotencyKey);

            return;
        }

        try
        {
            var usageDate = DateOnly.FromDateTime(message.OccurredAtUtc);

            await _usageService.IncrementAsync(message.ApiKeyId, usageDate, ct);

            await _idempotencyService.MarkProcessedAsync(
                message.ApiKeyId,
                message.IdempotencyKey,
                ct);

            _logger.LogInformation(
                "Usage event processed successfully. ApiKeyId: {ApiKeyId}, IdempotencyKey: {IdempotencyKey}",
                message.ApiKeyId,
                message.IdempotencyKey);
        }
        catch (Exception)
        {
            await _idempotencyService.MarkFailedAsync(
                message.ApiKeyId,
                message.IdempotencyKey,
                ct);

            throw;
        }
    }
}