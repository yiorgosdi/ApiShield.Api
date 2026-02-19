namespace ApiShield.Core;

// ApiKeyValidator on Core stays ONLY for testing, no use in runtime. 
public sealed class ApiKeyValidator
{
    private readonly IApiKeyStore _store;

    public ApiKeyValidator(IApiKeyStore store) => _store = store;

    public async Task ValidateOrThrowAsync(string? apiKey, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(apiKey))
            throw new ArgumentException("API key is missing.");

        if (apiKey.Length < 16)
            throw new ArgumentException("API key is too short.");

        var exists = await _store.ExistsAsync(apiKey, ct);
        if (!exists)
            throw new UnauthorizedAccessException("Invalid API key.");
    }
}
