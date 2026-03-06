using System.Net;
using System.Net.Http.Json;
using ApiShield.Core.Usage;
using FluentAssertions;

namespace ApiShield.IntegrationTests.Features.Usage;

public sealed class UsageEndpointsTests : IClassFixture<ApiFactory>
{
    private readonly HttpClient _client;
    private const string AdminKey = TestKeys.AdminKey;

    public UsageEndpointsTests(ApiFactory factory)
        => _client = factory.CreateClient();

    [Fact]
    public async Task When_api_key_is_missing_increment_is_unauthorized()
    {
        // Arrange
        var req = new HttpRequestMessage(HttpMethod.Post, "/secure/usage/increment");

        // Act
        var res = await _client.SendAsync(req);

        // Assert
        res.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task When_api_key_is_valid_increment_returns_new_count()
    {
        // Arrange
        var req = new HttpRequestMessage(HttpMethod.Post, "/secure/usage/increment");
        req.Headers.Add("X-API-Key", AdminKey);

        // Act
        var res = await _client.SendAsync(req);

        var raw = await res.Content.ReadAsStringAsync();
        Console.WriteLine(raw);


        var body = await res.Content.ReadFromJsonAsync<UsageIncrementResponse>();

        // Assert
        res.StatusCode.Should().Be(HttpStatusCode.OK);
        body.Should().NotBeNull();
        body!.NewCount.Should().BeGreaterThan(0);
    }

    [Fact]
    public async Task When_increment_called_twice_count_increases()
    {
        // Arrange
        var req1 = new HttpRequestMessage(HttpMethod.Post, "/secure/usage/increment");
        req1.Headers.Add("X-API-Key", AdminKey);

        var req2 = new HttpRequestMessage(HttpMethod.Post, "/secure/usage/increment");
        req2.Headers.Add("X-API-Key", AdminKey);

        // Act
        var res1 = await _client.SendAsync(req1);
        var body1 = await res1.Content.ReadFromJsonAsync<UsageIncrementResponse>();

        var res2 = await _client.SendAsync(req2);
        var body2 = await res2.Content.ReadFromJsonAsync<UsageIncrementResponse>();

        // Assert
        res1.StatusCode.Should().Be(HttpStatusCode.OK);
        res2.StatusCode.Should().Be(HttpStatusCode.OK);

        body1.Should().NotBeNull();
        body2.Should().NotBeNull();

        body2!.NewCount.Should().Be(body1!.NewCount + 1);
    }
}