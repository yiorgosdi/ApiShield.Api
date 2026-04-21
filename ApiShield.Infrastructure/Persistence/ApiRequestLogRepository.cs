using ApiShield.Core.Idempotency;
using Microsoft.EntityFrameworkCore;

namespace ApiShield.Infrastructure.Persistence;

public sealed class ApiRequestLogRepository : IApiRequestLogRepository
{
    private readonly ApiShieldDbContext _dbContext;

    public ApiRequestLogRepository(ApiShieldDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IdempotencyClaimResult> TryClaimAsync(
        string apiKeyId,
        string idempotencyKey,
        string route,
        CancellationToken ct)
    {
        var record = new ApiRequestLog
        {
            ApiKeyId = apiKeyId,
            IdempotencyKey = idempotencyKey,
            Route = route,
            CreatedAtUtc = DateTime.UtcNow,
            Status = RequestProcessingStatus.Pending
        };

        _dbContext.ApiRequestLogs.Add(record);

        try
        {
            await _dbContext.SaveChangesAsync(ct);
            return new IdempotencyClaimResult(Claimed: true);
        }
        catch (Exception ex) when (ex.IsUniqueConstraintViolation())
        {
            _dbContext.Entry(record).State = EntityState.Detached;

            var existing = await GetAsync(apiKeyId, idempotencyKey, ct);
            return new IdempotencyClaimResult(Claimed: false, ExistingRecord: existing);
        }
    }

    public Task<ApiRequestLog?> GetAsync(
        string apiKeyId,
        string idempotencyKey,
        CancellationToken ct)
    {
        return _dbContext.ApiRequestLogs
            .SingleOrDefaultAsync(
                x => x.ApiKeyId == apiKeyId && x.IdempotencyKey == idempotencyKey,
                ct);
    }

    public async Task MarkProcessedAsync(
        string apiKeyId,
        string idempotencyKey,
        DateTime processedAtUtc,
        CancellationToken ct)
    {
        var record = await _dbContext.ApiRequestLogs
            .SingleAsync(
                x => x.ApiKeyId == apiKeyId && x.IdempotencyKey == idempotencyKey,
                ct);

        record.Status = RequestProcessingStatus.Processed;
        record.ProcessedAtUtc = processedAtUtc;

        await _dbContext.SaveChangesAsync(ct);
    }

    public async Task MarkFailedAsync(
        string apiKeyId,
        string idempotencyKey,
        CancellationToken ct)
    {
        var record = await _dbContext.ApiRequestLogs
            .SingleAsync(
                x => x.ApiKeyId == apiKeyId && x.IdempotencyKey == idempotencyKey,
                ct);

        record.Status = RequestProcessingStatus.Failed;

        await _dbContext.SaveChangesAsync(ct);
    }
}