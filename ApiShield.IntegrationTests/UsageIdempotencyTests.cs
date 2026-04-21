using System.Net;
using ApiShield.IntegrationTests.Helper;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;

namespace ApiShield.IntegrationTests;

public sealed class UsageIdempotencyTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;
    private const string ApiKey = TestKeys.AdminKey;

    public UsageIdempotencyTests(WebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Increment_with_same_idempotency_key_twice_should_increment_only_once()
    {
        // Arrange
        var before = await UsageTestHelper.GetTodayUsageAsync(_client, ApiKey);
        var idempotencyKey = $"idem-{Guid.NewGuid():N}";

        var request1 = UsageRequestFactory.CreateIncrementRequest(ApiKey, idempotencyKey);
        var request2 = UsageRequestFactory.CreateIncrementRequest(ApiKey, idempotencyKey);

        // Act
        var response1 = await _client.SendAsync(request1);
        var response2 = await _client.SendAsync(request2);

        var after = await UsageTestHelper.WaitForUsageCountAsync(_client, ApiKey, before.Count + 1);

        // Assert
        response1.StatusCode.Should().Be(HttpStatusCode.Accepted);
        response2.StatusCode.Should().Be(HttpStatusCode.Accepted);
        after.Count.Should().Be(before.Count + 1);
    }

    public sealed record UsageTodayResponse(DateOnly UsageDate, int Count);
}