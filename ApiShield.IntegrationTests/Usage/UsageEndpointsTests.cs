using System.Net;
using System.Net.Http.Json;
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
    public async Task When_api_key_is_valid_increment_returns_accepted()
    {
        // Arrange
        var req = new HttpRequestMessage(HttpMethod.Post, "/secure/usage/increment");
        req.Headers.Add("X-API-Key", AdminKey);

        // Act
        var res = await _client.SendAsync(req);

        // Assert
        res.StatusCode.Should().Be(HttpStatusCode.Accepted);
    }

    [Fact]
    public async Task When_increment_called_usage_today_eventually_reflects_new_count()
    {
        // Arrange
        var before = await GetTodayUsageAsync();

        var req = new HttpRequestMessage(HttpMethod.Post, "/secure/usage/increment");
        req.Headers.Add("X-API-Key", AdminKey);

        // Act
        var postRes = await _client.SendAsync(req);

        // Assert enqueue accepted
        postRes.StatusCode.Should().Be(HttpStatusCode.Accepted);

        var after = await WaitForUsageCountAsync(before.Count + 1);

        after.Count.Should().Be(before.Count + 1);
        after.UsageDate.Should().Be(DateOnly.FromDateTime(DateTime.UtcNow));
    }

    [Fact]
    public async Task When_increment_called_twice_usage_today_eventually_increases_by_two()
    {
        // Arrange
        var before = await GetTodayUsageAsync();

        var req1 = new HttpRequestMessage(HttpMethod.Post, "/secure/usage/increment");
        req1.Headers.Add("X-API-Key", AdminKey);

        var req2 = new HttpRequestMessage(HttpMethod.Post, "/secure/usage/increment");
        req2.Headers.Add("X-API-Key", AdminKey);

        // Act
        var res1 = await _client.SendAsync(req1);
        var res2 = await _client.SendAsync(req2);

        // Assert enqueue accepted
        res1.StatusCode.Should().Be(HttpStatusCode.Accepted);
        res2.StatusCode.Should().Be(HttpStatusCode.Accepted);

        var after = await WaitForUsageCountAsync(before.Count + 2);

        after.Count.Should().Be(before.Count + 2);
        after.UsageDate.Should().Be(DateOnly.FromDateTime(DateTime.UtcNow));
    }

    private async Task<UsageTodayResponse> GetTodayUsageAsync()
    {
        var req = new HttpRequestMessage(HttpMethod.Get, "/secure/usage/today");
        req.Headers.Add("X-API-Key", AdminKey);

        var res = await _client.SendAsync(req);
        res.StatusCode.Should().Be(HttpStatusCode.OK);

        var body = await res.Content.ReadFromJsonAsync<UsageTodayResponse>();

        body.Should().NotBeNull();
        return body!;
    }

    private async Task<UsageTodayResponse> WaitForUsageCountAsync(int expectedCount)
    {
        const int maxAttempts = 20;
        var delay = TimeSpan.FromMilliseconds(150);

        for (var attempt = 0; attempt < maxAttempts; attempt++)
        {
            var current = await GetTodayUsageAsync();

            if (current.Count >= expectedCount)
            {
                return current;
            }

            await Task.Delay(delay);
        }

        var final = await GetTodayUsageAsync();
        final.Count.Should().BeGreaterThanOrEqualTo(expectedCount,
            "the background usage processor should eventually persist queued increments");

        return final;
    }

    public sealed record UsageTodayResponse(DateOnly UsageDate, int Count);
}