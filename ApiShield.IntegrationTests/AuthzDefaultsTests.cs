using System.Net;

namespace ApiShield.IntegrationTests;

public sealed class AuthzDefaultsTests : IClassFixture<ApiShieldWebApplicationFactory>
{
    private readonly HttpClient _client;

    public AuthzDefaultsTests(ApiShieldWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Admin_endpoint_with_basic_key_is_forbidden()
    {
        // Arrange
        var req = new HttpRequestMessage(HttpMethod.Get, "/secure/admin");
        req.Headers.Add(TestKeys.HeaderName, TestKeys.BasicKey);

        // Act
        var res = await _client.SendAsync(req);

        // Assert
        Assert.Equal(HttpStatusCode.Forbidden, res.StatusCode);
    }
}