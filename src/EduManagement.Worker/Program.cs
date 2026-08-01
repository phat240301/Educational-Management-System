using EduManagement.Infrastructure.Messaging;
using EduManagement.Worker.Consumers;
using EduManagement.Worker.Services;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.Configure<RabbitMqOptions>(builder.Configuration.GetSection(RabbitMqOptions.SectionName));
builder.Services.AddSingleton<RabbitMqConnectionProvider>();

builder.Services.AddSingleton<IEmailNotificationService, MockEmailNotificationService>();
builder.Services.AddHostedService<GradeUpdatedEventConsumer>();

var host = builder.Build();
host.Run();
