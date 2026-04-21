namespace ApiShield.Core.Idempotency;

public interface IApiRequestLogRepository
{
    Task<IdempotencyClaimResult> TryClaimAsync(
        string apiKeyId,
        string idempotencyKey,
        string route,
        CancellationToken ct);

    Task<ApiRequestLog?> GetAsync(
        string apiKeyId,
        string idempotencyKey,
        CancellationToken ct);

    Task MarkProcessedAsync(
        string apiKeyId,
        string idempotencyKey,
        DateTime processedAtUtc,
        CancellationToken ct);

    Task MarkFailedAsync(
        string apiKeyId,
        string idempotencyKey,
        CancellationToken ct);
}