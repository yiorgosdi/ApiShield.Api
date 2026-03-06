namespace ApiShield.Core.Usage;

public sealed record UsageIncrementResponse(DateOnly UsageDate, int NewCount);
public sealed record UsageTodayResponse(DateOnly UsageDate, int Count);