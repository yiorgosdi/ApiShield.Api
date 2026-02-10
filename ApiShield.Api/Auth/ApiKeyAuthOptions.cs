namespace ApiShield.Api.Auth;

public sealed class ApiKeyAuthOptions
{
    public List<ApiKeyEntry> Keys { get; set; } = new();
}

public sealed class ApiKeyEntry
{
    public string Key { get; set; } = default!;
    public string Role { get; set; } = default!;
}