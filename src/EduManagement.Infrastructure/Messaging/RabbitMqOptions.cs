namespace EduManagement.Infrastructure.Messaging;

public class RabbitMqOptions
{
    public const string SectionName = "RabbitMq";

    public string HostName { get; set; } = "localhost";
    public int Port { get; set; } = 5672;
    public string UserName { get; set; } = "edu_admin";
    public string Password { get; set; } = "changeme_local_only";
    public string ExchangeName { get; set; } = "edu.events";
}
