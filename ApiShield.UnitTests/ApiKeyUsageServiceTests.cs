using ApiShield.Infrastructure.Persistence;
using ApiShield.Infrastructure.Usage;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;

namespace ApiShield.UnitTests;

public class ApiKeyUsageServiceTests
{
    // following are Infrastructure-assisted unit tests.  
    [Fact]
    public async Task GetTodayAsync_WhenUsageRecordExists_ReturnsExistingCount()
    {
        const string validApiKey = "valid-api-key-1234567890abcdef";
        var timeProvider = TimeProvider.System;
        var date = new DateOnly(2026, 03, 26);

        // Arrange 
        var options = new DbContextOptionsBuilder<ApiShieldDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        await using var db = new ApiShieldDbContext(options);

        db.ApiKeyDailyUsage.Add(new ApiKeyDailyUsage
        {
            KeyId = validApiKey,
            UsageDate = date,
            Count = 5
        });

        await db.SaveChangesAsync();

        var service = new ApiKeyUsageService(db, timeProvider);

        // Act 
        var result = await service.GetTodayAsync(validApiKey, date, CancellationToken.None);

        // Assert
        result.Count.Should().Be(5);
        result.UsageDate.Should().Be(date);
    }

    // no record test. 
    [Fact]
    public async Task GetTodayAsync_WhenUsageRecordExists_ReturnsZeroCount()
    {
        const string validApiKey = "valid-api-key-1234567890abcdef";
        var timeProvider = TimeProvider.System;
        var date = new DateOnly(2026, 03, 26);

        // Arrange 
        var options = new DbContextOptionsBuilder<ApiShieldDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        await using var db = new ApiShieldDbContext(options);

        db.ApiKeyDailyUsage.Add(new ApiKeyDailyUsage
        {
            KeyId = validApiKey,
            UsageDate = date,
            Count = 0
        });

        await db.SaveChangesAsync();

        // Act 
        var service = new ApiKeyUsageService(db, timeProvider);

        // Assert
        var result = await service.GetTodayAsync(validApiKey, date, CancellationToken.None);

        result.Count.Should().Be(0);
        result.UsageDate.Should().Be(date);


        var numbers = new List<int> { 1, 2, 3, 4 };

        var query = numbers.Where(x => x > 2);

        numbers.Add(5);

        var result1 = query.ToList();

    }

    [Fact]
    public async Task GetTodayAsync_WhenUsageRecordDoesNotExist_ReturnsZeroCount()
    {
        const string validApiKey = "valid-api-key-1234567890abcdef";
        const string validApiKeyNonExisting = "valid-api-key-1234567890abcghi";
        var timeProvider = TimeProvider.System;
        var date = new DateOnly(2026, 03, 26);

        // Arrange 
        var options = new DbContextOptionsBuilder<ApiShieldDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        await using var db = new ApiShieldDbContext(options);

        db.ApiKeyDailyUsage.Add(new ApiKeyDailyUsage
        {
            KeyId = validApiKey,
            UsageDate = date,
            Count = 1
        });

        await db.SaveChangesAsync();

        // Act 
        var service = new ApiKeyUsageService(db, timeProvider);

        // Assert
        var result = await service.GetTodayAsync(validApiKeyNonExisting, date, CancellationToken.None);

        result.Count.Should().Be(0);
        result.UsageDate.Should().Be(date);


        string s = "hello";
        s = s.ToUpper();
        Console.WriteLine(s);
    }
}
