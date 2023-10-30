using CQRS.Core.Consumers;

namespace Post.Query.Api.Consumers;

public class ConsumerHostedService : IHostedService
{
    private readonly ILogger<ConsumerHostedService> _logger;
    private readonly IServiceProvider _serviceProvider;

    public ConsumerHostedService(ILogger<ConsumerHostedService> logger, IServiceProvider serviceProvider) {
        _logger = logger;
        _serviceProvider = serviceProvider;
    }

    public Task StartAsync(CancellationToken cancellationToken) {
        _logger.LogInformation("Event consumer service is running.");

        using var scope = _serviceProvider.CreateScope();

        var eventConsumer = scope.ServiceProvider.GetRequiredService<IEventConsumer>();
        var topic = Environment.GetEnvironmentVariable("KAFKA_TOPIC")
            ?? throw new ApplicationException("Environment variable [KAFKA_TOPIC] is not defined");
        Task.Run(() => eventConsumer.Consume(topic), cancellationToken);
            

        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken) {
        _logger.LogInformation("Event consumer service stopped.");
        return Task.CompletedTask;
    }
}
