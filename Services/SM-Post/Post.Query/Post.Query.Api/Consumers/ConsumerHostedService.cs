using CQRS.Core.Consumers;

namespace Post.Query.Api.Consumers;

public class ConsumerHostedService : IHostedService
{
    private readonly ILogger<ConsumerHostedService> _logger;
    private readonly IServiceProvider _serviceProvider;
    private readonly IEventConsumer _eventConsumer;

    public ConsumerHostedService(ILogger<ConsumerHostedService> logger, IServiceProvider serviceProvider, IEventConsumer eventConsumer) {
        _logger = logger;
        _serviceProvider = serviceProvider;
        _eventConsumer = eventConsumer;
    }

    public Task StartAsync(CancellationToken cancellationToken) {
        _logger.LogInformation("Event consumer service is running.");
        var topic = Environment.GetEnvironmentVariable("KAFKA_TOPIC")
            ?? throw new ApplicationException("Environment variable [KAFKA_TOPIC] is not defined");
        Task.Run(() => _eventConsumer.Consume(topic), cancellationToken);
            

        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken) {
        _logger.LogInformation("Event consumer service stopped.");
        return Task.CompletedTask;
    }
}
