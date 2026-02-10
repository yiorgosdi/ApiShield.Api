namespace ApiShield.IntegrationTests;

public static class TestKeys
{
    public const string HeaderName = "X-API-Key";

    // Keys aligned with ApiShield.Api appsettings.Development.json
    public const string AdminKey = "1234567890abcdef1234567890abcdef";
    public const string BasicKey = "bbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbb";

    // Well-formed but NOT present in config/store
    public const string NotPresentKey = "1234567890abcdef1234567890abcabc";
}
