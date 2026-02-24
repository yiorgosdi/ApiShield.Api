using System.Net;

namespace ApiShield.IntegrationTests;

public sealed class SecurityDefaultsTests : IClassFixture<ApiShieldWebApplicationFactory>
{
    private readonly HttpClient _client;

    public SecurityDefaultsTests(ApiShieldWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Secure_ping_without_api_key_is_unauthorized()
    {
        var res = await _client.GetAsync("/secure/ping");

        Assert.Equal(HttpStatusCode.Unauthorized, res.StatusCode);
    }
}