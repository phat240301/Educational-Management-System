using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;

namespace EduManagement.Infrastructure.Messaging;

// Lazily creates a single shared connection and retries with exponential backoff on failure,
// so a RabbitMQ outage at startup doesn't crash the Api/Worker host.
public class RabbitMqConnectionProvider(
    IOptions<RabbitMqOptions> options,
    ILogger<RabbitMqConnectionProvider> logger) : IAsyncDisposable
{
    private const int MaxConnectAttempts = 5;

    private readonly SemaphoreSlim _lock = new(1, 1);
    private IConnection? _connection;

    public async Task<IConnection> GetConnectionAsync(CancellationToken ct = default)
    {
        if (_connection is { IsOpen: true })
        {
            return _connection;
        }

        await _lock.WaitAsync(ct);
        try
        {
            if (_connection is { IsOpen: true })
            {
                return _connection;
            }

            var factory = new ConnectionFactory
            {
                HostName = options.Value.HostName,
                Port = options.Value.Port,
                UserName = options.Value.UserName,
                Password = options.Value.Password
            };

            for (var attempt = 1; attempt <= MaxConnectAttempts; attempt++)
            {
                try
                {
                    _connection = await factory.CreateConnectionAsync(ct);
                    logger.LogInformation("Connected to RabbitMQ at {HostName}:{Port}", options.Value.HostName, options.Value.Port);
                    return _connection;
                }
                catch (Exception ex) when (attempt < MaxConnectAttempts)
                {
                    var delay = TimeSpan.FromSeconds(Math.Pow(2, attempt));
                    logger.LogWarning(ex,
                        "RabbitMQ connection attempt {Attempt}/{MaxAttempts} failed. Retrying in {Delay}s.",
                        attempt, MaxConnectAttempts, delay.TotalSeconds);
                    await Task.Delay(delay, ct);
                }
            }

            _connection = await factory.CreateConnectionAsync(ct);
            return _connection;
        }
        finally
        {
            _lock.Release();
        }
    }

    public bool IsHealthy => _connection is { IsOpen: true };

    public async ValueTask DisposeAsync()
    {
        if (_connection is not null)
        {
            await _connection.CloseAsync();
            await _connection.DisposeAsync();
        }

        _lock.Dispose();
    }
}
