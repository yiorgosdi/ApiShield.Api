using System.Net;
using System.Net.Http.Json;
using FluentAssertions;

namespace ApiShield.IntegrationTests.Helper;

public static class UsageTestHelper
{
    public static async Task<UsageTodayResponse> GetTodayUsageAsync(HttpClient client, string apiKey)
    {
        var req = UsageRequestFactory.CreateTodayRequest(apiKey);

        var res = await client.SendAsync(req);
        res.StatusCode.Should().Be(HttpStatusCode.OK);

        var body = await res.Content.ReadFromJsonAsync<UsageTodayResponse>();
        body.Should().NotBeNull();

        return body!;
    }

    public static async Task<UsageTodayResponse> WaitForUsageCountAsync(
        HttpClient client,
        string apiKey,
        int expectedCount)
    {
        const int maxAttempts = 20;
        var delay = TimeSpan.FromMilliseconds(150);

        for (var attempt = 0; attempt < maxAttempts; attempt++)
        {
            var current = await GetTodayUsageAsync(client, apiKey);

            if (current.Count >= expectedCount)
            {
                return current;
            }

            await Task.Delay(delay);
        }

        var final = await GetTodayUsageAsync(client, apiKey);
        final.Count.Should().BeGreaterThanOrEqualTo(expectedCount,
            "the background usage processor should eventually persist queued increments");

        return final;
    }

    public sealed record UsageTodayResponse(DateOnly UsageDate, int Count);
}