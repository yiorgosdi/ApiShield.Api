using System.Threading.Channels;
using ApiShield.Api.Messaging;

namespace ApiShield.Api.Infrastructure.Queue;

public sealed class InMemoryUsageEventQueue : IUsageEventQueue
{
    private readonly Channel<UsageIncrementRequested> _channel;

    public InMemoryUsageEventQueue(Channel<UsageIncrementRequested> channel)
    {
        _channel = channel;
    }

    public async Task EnqueueAsync(
        UsageIncrementRequested message,
        CancellationToken cancellationToken = default)
    {
        await _channel.Writer.WriteAsync(message, cancellationToken);
    }
}