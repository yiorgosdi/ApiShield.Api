using System.Net;
using System.Net.Http.Json;
using ApiShield.IntegrationTests.Helper;
using FluentAssertions;

namespace ApiShield.IntegrationTests.Usage;

/* 
 * OPEN DOCKER! ApiFactory > SQL Server container via Testcontainers,
 */

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
        var request = new HttpRequestMessage(HttpMethod.Post, "/secure/usage/increment");
        request.Headers.Add("X-Idempotency-Key", "test-1");

        // Act
        var res = await _client.SendAsync(request);

        // Assert
        res.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task When_api_key_is_valid_increment_returns_accepted()
    {
        // Arrange
        var request = UsageRequestFactory.CreateIncrementRequest(AdminKey, "test-1");

        // Act
        var res = await _client.SendAsync(request);

        // Assert
        res.StatusCode.Should().Be(HttpStatusCode.Accepted);
    }

    [Fact]
    public async Task When_increment_called_usage_today_eventually_reflects_new_count()
    {
        // Arrange
        var before = await UsageTestHelper.GetTodayUsageAsync(_client, AdminKey);
        var request = UsageRequestFactory.CreateIncrementRequest(AdminKey, Guid.NewGuid().ToString("N"));

        // Act
        var postRes = await _client.SendAsync(request);

        // Assert
        postRes.StatusCode.Should().Be(HttpStatusCode.Accepted);

        var after = await UsageTestHelper.WaitForUsageCountAsync(_client, AdminKey, before.Count + 1);

        after.Count.Should().Be(before.Count + 1);
        after.UsageDate.Should().Be(DateOnly.FromDateTime(DateTime.UtcNow));
    }

    [Fact]
    public async Task When_increment_called_twice_with_different_idempotency_keys_usage_today_eventually_increases_by_two()
    {
        // Arrange
        var before = await UsageTestHelper.GetTodayUsageAsync(_client, AdminKey);

        var req1 = UsageRequestFactory.CreateIncrementRequest(AdminKey, "test-1");
        var req2 = UsageRequestFactory.CreateIncrementRequest(AdminKey, "test-2");

        // Act
        var res1 = await _client.SendAsync(req1);
        var res2 = await _client.SendAsync(req2);

        // Assert
        res1.StatusCode.Should().Be(HttpStatusCode.Accepted);
        res2.StatusCode.Should().Be(HttpStatusCode.Accepted);

        var after = await UsageTestHelper.WaitForUsageCountAsync(_client, AdminKey, before.Count + 2);

        after.Count.Should().Be(before.Count + 2);
        after.UsageDate.Should().Be(DateOnly.FromDateTime(DateTime.UtcNow));
    }

    [Fact]
    public async Task When_usage_today_is_requested_it_returns_ok_with_non_negative_count()
    {
        // Arrange
        var req = UsageRequestFactory.CreateTodayRequest(AdminKey);

        // Act
        var res = await _client.SendAsync(req);

        // Assert
        res.StatusCode.Should().Be(HttpStatusCode.OK);

        var body = await res.Content.ReadFromJsonAsync<UsageTodayResponse>();
        body.Should().NotBeNull();
        body!.Count.Should().BeGreaterThanOrEqualTo(0);
    }   

    public sealed record UsageTodayResponse(DateOnly UsageDate, int Count);
}