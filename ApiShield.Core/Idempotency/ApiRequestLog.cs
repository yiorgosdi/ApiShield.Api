namespace ApiShield.Core.Idempotency;

public sealed class ApiRequestLog
{
    public long Id { get; set; }

    public string IdempotencyKey { get; set; } = null!;

    public string ApiKeyId { get; set; } = null!;

    public string Route { get; set; } = null!;

    public DateTime CreatedAtUtc { get; set; }

    public DateTime? ProcessedAtUtc { get; set; }

    public string Status { get; set; } = null!;
}