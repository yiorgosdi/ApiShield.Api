namespace ApiShield.Core.Idempotency;

public interface IIdempotencyService
{
    Task<IdempotencyDecision> TryStartRequestAsync(
        string apiKeyId,
        string idempotencyKey,
        string route,
        CancellationToken ct);

    Task MarkProcessedAsync(
        string apiKeyId,
        string idempotencyKey,
        CancellationToken ct);

    Task MarkFailedAsync(
        string apiKeyId,
        string idempotencyKey,
        CancellationToken ct);

    Task<bool> IsAlreadyProcessedAsync(
        string apiKeyId,
        string idempotencyKey,
        CancellationToken ct);
}