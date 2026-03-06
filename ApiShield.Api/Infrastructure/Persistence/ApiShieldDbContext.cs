using Microsoft.EntityFrameworkCore;

namespace ApiShield.Api.Infrastructure.Persistence;

public sealed class ApiShieldDbContext : DbContext
{
    public ApiShieldDbContext(DbContextOptions<ApiShieldDbContext> options)
        : base(options) { }

    public DbSet<ApiKeyDailyUsage> ApiKeyDailyUsage => Set<ApiKeyDailyUsage>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ApiKeyDailyUsage>()
            .HasKey(x => new { x.KeyId, x.UsageDate });
    }
}

public sealed class ApiKeyDailyUsage
{
    public string KeyId { get; set; } = default!;
    public DateOnly UsageDate { get; set; }   // DATE only semantics
    public int Count { get; set; }
    public DateTime UpdatedAtUtc { get; set; }
}