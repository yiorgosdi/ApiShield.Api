namespace ApiShield.Core.Idempotency;

public sealed record IdempotencyDecision(
    bool ShouldEnqueue,
    string CurrentStatus);