namespace ApiShield.Core
{
    public interface IApiKeyStore
    {
        Task<bool> ExistsAsync(string apiKey, CancellationToken ct);
    }
}
