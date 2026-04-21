namespace ApiShield.Core.Idempotency;

public sealed class IdempotencyService : IIdempotencyService
{
    private readonly IApiRequestLogRepository _repository;

    public IdempotencyService(IApiRequestLogRepository repository)
    {
        _repository = repository;
    }

    public async Task<IdempotencyDecision> TryStartRequestAsync(
        string apiKeyId,
        string idempotencyKey,
        string route,
        CancellationToken ct)
    {
        var claim = await _repository.TryClaimAsync(apiKeyId, idempotencyKey, route, ct);

        if (claim.Claimed)
        {
            return new IdempotencyDecision(
                ShouldEnqueue: true,
                CurrentStatus: RequestProcessingStatus.Pending);
        }

        var existingStatus = claim.ExistingRecord?.Status ?? RequestProcessingStatus.Pending;

        return new IdempotencyDecision(
            ShouldEnqueue: false,
            CurrentStatus: existingStatus);
    }

    public Task MarkProcessedAsync(
        string apiKeyId,
        string idempotencyKey,
        CancellationToken ct)
        => _repository.MarkProcessedAsync(apiKeyId, idempotencyKey, DateTime.UtcNow, ct);

    public Task MarkFailedAsync(
        string apiKeyId,
        string idempotencyKey,
        CancellationToken ct)
        => _repository.MarkFailedAsync(apiKeyId, idempotencyKey, ct);

    public async Task<bool> IsAlreadyProcessedAsync(
        string apiKeyId,
        string idempotencyKey,
        CancellationToken ct)
    {
        var existing = await _repository.GetAsync(apiKeyId, idempotencyKey, ct);
        return existing?.Status == RequestProcessingStatus.Processed;
    }
}