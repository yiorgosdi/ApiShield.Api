using ApiShield.Core.Idempotency;
using Microsoft.EntityFrameworkCore;

namespace ApiShield.Infrastructure.Persistence;

public sealed class ApiShieldDbContext : DbContext
{
    public ApiShieldDbContext(DbContextOptions<ApiShieldDbContext> options)
        : base(options)
    {
    }

    public DbSet<ApiKeyDailyUsage> ApiKeyDailyUsage => Set<ApiKeyDailyUsage>();

    public DbSet<ApiRequestLog> ApiRequestLogs => Set<ApiRequestLog>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<ApiRequestLog>(builder =>
        {
            modelBuilder.Entity<ApiKeyDailyUsage>()
           .HasKey(x => new { x.KeyId, x.UsageDate });

            builder.ToTable("ApiRequestLog");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id)
                .ValueGeneratedOnAdd();

            builder.Property(x => x.IdempotencyKey)
                .HasMaxLength(100)
                .IsRequired();

            builder.Property(x => x.ApiKeyId)
                .HasMaxLength(100)
                .IsRequired();

            builder.Property(x => x.Route)
                .HasMaxLength(200)
                .IsRequired();

            builder.Property(x => x.CreatedAtUtc)
                .HasColumnType("datetime2")
                .IsRequired();

            builder.Property(x => x.ProcessedAtUtc)
                .HasColumnType("datetime2");

            builder.Property(x => x.Status)
                .HasMaxLength(20)
                .IsRequired();

            builder.HasIndex(x => new { x.ApiKeyId, x.IdempotencyKey })
                .IsUnique()
                .HasDatabaseName("UX_ApiRequestLog_ApiKeyId_IdempotencyKey");
        });
    }
}

public sealed class ApiKeyDailyUsage
//public class ApiKeyDailyUsage
{
    public string KeyId { get; set; } = default!;
    public DateOnly UsageDate { get; set; }   // DATE only semantics
    public int Count { get; set; }
    public DateTime UpdatedAtUtc { get; set; }
}