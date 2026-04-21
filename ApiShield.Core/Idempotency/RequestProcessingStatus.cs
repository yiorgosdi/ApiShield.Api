namespace ApiShield.Core.Idempotency;

public static class RequestProcessingStatus
{
    public const string Pending = "Pending";
    public const string Processed = "Processed";
    public const string Failed = "Failed";
}
