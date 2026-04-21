namespace ApiShield.Core.Idempotency;

public sealed record IdempotencyClaimResult(bool Claimed, ApiRequestLog? ExistingRecord = null);

/* “I modeled the idempotency claim as a result object that captures both success and the existing request state in case of duplicates, 
 * instead of returning a boolean.This allows the application layer to make decisions based on the current lifecycle state of the request.” 
 */