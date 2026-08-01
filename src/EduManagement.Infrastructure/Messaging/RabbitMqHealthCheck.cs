using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace EduManagement.Infrastructure.Messaging;

public class RabbitMqHealthCheck(RabbitMqConnectionProvider connectionProvider) : IHealthCheck
{
    public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context, CancellationToken ct = default)
    {
        try
        {
            var connection = await connectionProvider.GetConnectionAsync(ct);
            return connection.IsOpen
                ? HealthCheckResult.Healthy("RabbitMQ connection is open.")
                : HealthCheckResult.Unhealthy("RabbitMQ connection is not open.");
        }
        catch (Exception ex)
        {
            return HealthCheckResult.Unhealthy("Unable to reach RabbitMQ.", ex);
        }
    }
}
