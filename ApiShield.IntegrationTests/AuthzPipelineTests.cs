using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;
using System.Text.Json;

namespace ApiShield.IntegrationTests;

public class AuthzPipelineTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;
    
    public AuthzPipelineTests(WebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task When_api_key_is_basic_requesting_admin_endpoint_is_forbidden()
    {
        var req = new HttpRequestMessage(HttpMethod.Get, "/secure/admin");
        req.Headers.Add(TestKeys.HeaderName, TestKeys.BasicKey);

        var res = await _client.SendAsync(req);

        Assert.NotEqual(HttpStatusCode.Unauthorized, res.StatusCode);
        Assert.Equal(HttpStatusCode.Forbidden, res.StatusCode);
    }

    [Fact]
    public async Task When_api_key_is_admin_requesting_admin_endpoint_succeeds()
    {
        var expectedContent = "admin pong";
        var req = new HttpRequestMessage(HttpMethod.Get, "/secure/admin");
        req.Headers.Add(TestKeys.HeaderName, TestKeys.AdminKey);  

        var res = await _client.SendAsync(req);

        Assert.Equal(HttpStatusCode.OK, res.StatusCode);

        var body = await res.Content.ReadAsStringAsync();
        using var json = JsonDocument.Parse(body);
        var message = json.RootElement.GetProperty("message").GetString();
        Assert.Equal(expectedContent, message);
    }
}
