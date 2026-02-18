using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;
using System.Text.Json;

namespace ApiShield.IntegrationTests;

public class AuthnPipelineTests : IClassFixture<ApiShieldWebApplicationFactory>
{
    private readonly HttpClient _client;
    
    public AuthnPipelineTests(ApiShieldWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task When_api_key_header_is_missing_request_is_unauthorized()
    {
        var res = await _client.GetAsync("/secure/ping");

        Assert.Equal(HttpStatusCode.Unauthorized, res.StatusCode);//401
    }

    [Fact]
    public async Task When_api_key_is_not_present_request_is_unauthorized()
    {
        // Arrange
        var req = new HttpRequestMessage(HttpMethod.Get, "/secure/ping");
        req.Headers.Add(TestKeys.HeaderName, TestKeys.NotPresentKey);

        // Act
        var res = await _client.SendAsync(req);

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, res.StatusCode);//401
    }

    [Fact]
    public async Task When_api_key_is_valid_request_succeeds()
    {
        // Arrange
        var expectedContent = "pong";

        var req = new HttpRequestMessage(HttpMethod.Get, "/secure/ping");
        req.Headers.Add(TestKeys.HeaderName, TestKeys.AdminKey);

        // Act
        var res = await _client.SendAsync(req);
        var body = await res.Content.ReadAsStringAsync();

        // Assert
        using var json = JsonDocument.Parse(body);
        var message = json.RootElement.GetProperty("message").GetString();

        Assert.Equal(HttpStatusCode.OK, res.StatusCode);
        Assert.Equal(expectedContent, message);    
    }
}
