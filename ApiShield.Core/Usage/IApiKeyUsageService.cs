namespace ApiShield.Core.Usage;

public interface IApiKeyUsageService
{
    Task<UsageIncrementResponse> IncrementAsync(string keyId, DateOnly date, CancellationToken ct);
    Task<UsageTodayResponse> GetTodayAsync(string keyId, DateOnly date, CancellationToken ct);
}