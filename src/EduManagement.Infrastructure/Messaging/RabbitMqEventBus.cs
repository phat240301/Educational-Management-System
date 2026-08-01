using System.Text.Json;
using EduManagement.Core.Application.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;

namespace EduManagement.Infrastructure.Messaging;

public class RabbitMqEventBus(
    RabbitMqConnectionProvider connectionProvider,
    IOptions<RabbitMqOptions> options,
    ILogger<RabbitMqEventBus> logger) : IEventBus
{
    private const int MaxPublishAttempts = 3;

    public async Task PublishAsync<TEvent>(TEvent @event, CancellationToken ct = default) where TEvent : class
    {
        var routingKey = typeof(TEvent).Name;
        var body = JsonSerializer.SerializeToUtf8Bytes(@event);

        for (var attempt = 1; attempt <= MaxPublishAttempts; attempt++)
        {
            try
            {
                var connection = await connectionProvider.GetConnectionAsync(ct);
                await using var channel = await connection.CreateChannelAsync(cancellationToken: ct);

                await channel.ExchangeDeclareAsync(
                    exchange: options.Value.ExchangeName,
                    type: ExchangeType.Topic,
                    durable: true,
                    autoDelete: false,
                    cancellationToken: ct);

                var properties = new BasicProperties
                {
                    ContentType = "application/json",
                    DeliveryMode = DeliveryModes.Persistent
                };

                await channel.BasicPublishAsync(
                    exchange: options.Value.ExchangeName,
                    routingKey: routingKey,
                    mandatory: false,
                    basicProperties: properties,
                    body: body,
                    cancellationToken: ct);

                logger.LogInformation("Published {EventType} (routing key {RoutingKey})", typeof(TEvent).Name, routingKey);
                return;
            }
            catch (Exception ex) when (attempt < MaxPublishAttempts)
            {
                var delay = TimeSpan.FromSeconds(attempt);
                logger.LogWarning(ex,
                    "Failed to publish {EventType}, attempt {Attempt}/{MaxAttempts}. Retrying in {Delay}s.",
                    typeof(TEvent).Name, attempt, MaxPublishAttempts, delay.TotalSeconds);
                await Task.Delay(delay, ct);
            }
        }
    }
}
