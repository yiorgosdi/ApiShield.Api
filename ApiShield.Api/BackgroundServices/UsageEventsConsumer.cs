using System.Threading.Channels;
using ApiShield.Api.Messaging;

public sealed class UsageEventsConsumer : BackgroundService
{
    private readonly Channel<UsageIncrementRequested> _channel;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<UsageEventsConsumer> _logger;

    public UsageEventsConsumer(
        Channel<UsageIncrementRequested> channel,
        IServiceScopeFactory scopeFactory,
        ILogger<UsageEventsConsumer> logger)
    {
        _channel = channel;
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("UsageEventsConsumer started");

        await foreach (var message in _channel.Reader.ReadAllAsync(stoppingToken))
        {
            try
            {
                using var scope = _scopeFactory.CreateScope();

                var processor =
                    scope.ServiceProvider.GetRequiredService<IUsageProcessingService>();

                await processor.ProcessAsync(message, stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Failed processing usage event {EventId}",
                    message.EventId);
            }
        }
    }
}