using ApiShield.Core.Usage;
using ApiShield.Infrastructure.Persistence;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace ApiShield.Infrastructure.Usage;

public sealed class ApiKeyUsageService : IApiKeyUsageService
{
    private readonly ApiShieldDbContext _db;
    private readonly TimeProvider _time;

    public ApiKeyUsageService(ApiShieldDbContext db, TimeProvider time)
    {
        _db = db;
        _time = time;
    }

    public async Task<UsageIncrementResponse> IncrementAsync(string keyId, DateOnly date, CancellationToken ct)
    {
        const int maxAttempts = 3;

        for (var attempt = 1; attempt <= maxAttempts; attempt++)
        {
            try
            {
                await using var tx = await _db.Database.BeginTransactionAsync(ct);

                var updatedAtUtc = _time.GetUtcNow().UtcDateTime;

                // UPDATE then INSERT-if-missed (atomic under tx).
                // UPDLOCK/HOLDLOCK reduces race in concurrent requests.
                var sql = @"
UPDATE dbo.ApiKeyDailyUsage WITH (UPDLOCK, HOLDLOCK)
SET [Count] = [Count] + 1,
    UpdatedAtUtc = @UpdatedAtUtc
WHERE KeyId = @KeyId AND UsageDate = @UsageDate;

IF @@ROWCOUNT = 0
BEGIN
    INSERT INTO dbo.ApiKeyDailyUsage (KeyId, UsageDate, [Count], UpdatedAtUtc)
    VALUES (@KeyId, @UsageDate, 1, @UpdatedAtUtc);
END;

SELECT [Count] AS [Value]
FROM dbo.ApiKeyDailyUsage
WHERE KeyId = @KeyId AND UsageDate = @UsageDate;
";

                // DateOnly -> DateTime for parameter (SQL date)
                var pKeyId = new SqlParameter("@KeyId", keyId);
                var pUsageDate = new SqlParameter("@UsageDate", date.ToDateTime(TimeOnly.MinValue));
                var pUpdatedAt = new SqlParameter("@UpdatedAtUtc", updatedAtUtc);

                // EF Core 8: SqlQueryRaw<T> for scalar/rows
                var result = await _db.Database
                    .SqlQueryRaw<int>(sql, pKeyId, pUsageDate, pUpdatedAt)
                    .ToListAsync(ct);

                var newCount = result.SingleOrDefault();

                await tx.CommitAsync(ct);

                return new UsageIncrementResponse(date, newCount);
            }
            catch (SqlException ex) when (ex.Number == 1205 && attempt < maxAttempts)
            {
                // deadlock victim -> backoff + retry
                await Task.Delay(TimeSpan.FromMilliseconds(50 * attempt), ct);
            }
        }

        throw new InvalidOperationException("Increment retry attempts exhausted.");
    }

    public async Task<UsageTodayResponse> GetTodayAsync(string keyId, DateOnly dt, CancellationToken ct)
    {
        var count = await _db.ApiKeyDailyUsage
            .AsNoTracking()
            .Where(x => x.KeyId == keyId && x.UsageDate == dt)
            .SumAsync(x => (int?)x.Count, ct) ?? 0;

        return new UsageTodayResponse(dt, count);
    }
}