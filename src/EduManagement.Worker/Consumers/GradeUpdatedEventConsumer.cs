using System.Text;
using System.Text.Json;
using EduManagement.Core.Application.Events;
using EduManagement.Infrastructure.Messaging;
using EduManagement.Worker.Services;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace EduManagement.Worker.Consumers;

// Consumes GradeUpdatedEvent from the "edu.events" topic exchange and triggers a
// (mocked) parent notification email. Failed deliveries are retried up to
// MaxRetries times with a header-based counter before being routed to a dead-letter
// queue, so a poison message can't block the queue forever.
public class GradeUpdatedEventConsumer(
    RabbitMqConnectionProvider connectionProvider,
    IOptions<RabbitMqOptions> options,
    IEmailNotificationService emailService,
    ILogger<GradeUpdatedEventConsumer> logger) : BackgroundService
{
    private const string QueueName = "notifications.grade-updated";
    private const string DeadLetterQueueName = "notifications.grade-updated.dlq";
    private const string RetryCountHeader = "x-retry-count";
    private const int MaxRetries = 3;

    private IChannel? _channel;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var connection = await connectionProvider.GetConnectionAsync(stoppingToken);
        _channel = await connection.CreateChannelAsync(cancellationToken: stoppingToken);

        await _channel.ExchangeDeclareAsync(
            exchange: options.Value.ExchangeName,
            type: ExchangeType.Topic,
            durable: true,
            autoDelete: false,
            cancellationToken: stoppingToken);

        await _channel.QueueDeclareAsync(QueueName, durable: true, exclusive: false, autoDelete: false, cancellationToken: stoppingToken);
        await _channel.QueueDeclareAsync(DeadLetterQueueName, durable: true, exclusive: false, autoDelete: false, cancellationToken: stoppingToken);
        await _channel.QueueBindAsync(QueueName, options.Value.ExchangeName, routingKey: nameof(GradeUpdatedEvent), cancellationToken: stoppingToken);

        await _channel.BasicQosAsync(prefetchSize: 0, prefetchCount: 10, global: false, cancellationToken: stoppingToken);

        var consumer = new AsyncEventingBasicConsumer(_channel);
        consumer.ReceivedAsync += OnMessageReceivedAsync;

        await _channel.BasicConsumeAsync(QueueName, autoAck: false, consumer, cancellationToken: stoppingToken);

        logger.LogInformation("Listening for {EventType} on queue {Queue}", nameof(GradeUpdatedEvent), QueueName);

        await Task.Delay(Timeout.Infinite, stoppingToken).ContinueWith(_ => { }, TaskScheduler.Default);
    }

    private async Task OnMessageReceivedAsync(object sender, BasicDeliverEventArgs ea)
    {
        var channel = (IChannel)((AsyncEventingBasicConsumer)sender).Channel;
        var retryCount = GetRetryCount(ea.BasicProperties.Headers);

        try
        {
            var gradeEvent = JsonSerializer.Deserialize<GradeUpdatedEvent>(ea.Body.Span)
                ?? throw new InvalidOperationException("Message body deserialized to null.");

            await emailService.SendGradeUpdatedNotificationAsync(gradeEvent);
            await channel.BasicAckAsync(ea.DeliveryTag, multiple: false);
        }
        catch (Exception ex)
        {
            if (retryCount < MaxRetries)
            {
                logger.LogWarning(ex,
                    "Failed to process {EventType} (delivery tag {Tag}), retry {Retry}/{MaxRetries}.",
                    nameof(GradeUpdatedEvent), ea.DeliveryTag, retryCount + 1, MaxRetries);

                await Task.Delay(TimeSpan.FromSeconds(Math.Pow(2, retryCount + 1)));
                await RepublishAsync(channel, QueueName, ea, retryCount + 1);
            }
            else
            {
                logger.LogError(ex,
                    "Giving up on {EventType} (delivery tag {Tag}) after {MaxRetries} retries. Routing to DLQ.",
                    nameof(GradeUpdatedEvent), ea.DeliveryTag, MaxRetries);

                await RepublishAsync(channel, DeadLetterQueueName, ea, retryCount);
            }

            await channel.BasicAckAsync(ea.DeliveryTag, multiple: false);
        }
    }

    private static async Task RepublishAsync(IChannel channel, string queueName, BasicDeliverEventArgs ea, int retryCount)
    {
        var properties = new BasicProperties
        {
            ContentType = ea.BasicProperties.ContentType,
            DeliveryMode = DeliveryModes.Persistent,
            Headers = new Dictionary<string, object?>(ea.BasicProperties.Headers ?? new Dictionary<string, object?>())
            {
                [RetryCountHeader] = retryCount
            }
        };

        await channel.BasicPublishAsync(
            exchange: string.Empty,
            routingKey: queueName,
            mandatory: false,
            basicProperties: properties,
            body: ea.Body,
            cancellationToken: CancellationToken.None);
    }

    private static int GetRetryCount(IDictionary<string, object?>? headers)
    {
        if (headers is null || !headers.TryGetValue(RetryCountHeader, out var value) || value is null)
        {
            return 0;
        }

        return value switch
        {
            int i => i,
            long l => (int)l,
            byte[] bytes => int.Parse(Encoding.UTF8.GetString(bytes)),
            _ => 0
        };
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        if (_channel is not null)
        {
            await _channel.CloseAsync(cancellationToken);
        }

        await base.StopAsync(cancellationToken);
    }
}
