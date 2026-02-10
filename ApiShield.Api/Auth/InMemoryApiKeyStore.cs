using ApiShield.Core;
using Microsoft.Extensions.Options;

namespace ApiShield.Api.Auth;

sealed class InMemoryApiKeyStore : IApiKeyStore
{
    private readonly ApiKeyAuthOptions _options;

    public InMemoryApiKeyStore(IOptions<ApiKeyAuthOptions> options)
        => _options = options.Value;

    public Task<bool> ExistsAsync(string apiKey, CancellationToken ct)
        => Task.FromResult(_options.Keys.Any(k => k.Key == apiKey));
}