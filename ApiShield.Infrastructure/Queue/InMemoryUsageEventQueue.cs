using ApiShield.Core.Usage;
using System.Threading.Channels;

namespace ApiShield.Infrastructure.Queue;

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