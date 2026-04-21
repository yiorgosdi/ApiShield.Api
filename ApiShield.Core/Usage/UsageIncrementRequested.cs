namespace ApiShield.Core.Usage;

public sealed record UsageIncrementRequested(
    Guid EventId,
    string ApiKeyId,
    string Route,
    DateTime OccurredAtUtc,
    string CorrelationId,
    string IdempotencyKey);

/* reminder: 
    field         		reason       
    ------------- 	----------------- 
    EventId       	idempotency later  
    ApiKey        	usage aggregation  
    Route         	analytics          
    OccurredAtUtc 	audit trail      
    CorrelationId 	tracing        
*/