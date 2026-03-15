namespace ApiShield.Api.Messaging;

public sealed record UsageIncrementRequested(
    Guid EventId,
    string ApiKey,
    string Route,
    DateTime OccurredAtUtc,
    string CorrelationId);

/* reminder: 
    field         		reason       
    ------------- 	----------------- 
    EventId       	idempotency later  
    ApiKey        	usage aggregation  
    Route         	analytics          
    OccurredAtUtc 	audit trail      
    CorrelationId 	tracing        
*/