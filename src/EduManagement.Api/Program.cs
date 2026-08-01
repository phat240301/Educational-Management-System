using EduManagement.Api.Filters;
using EduManagement.Api.Middleware;
using EduManagement.Core.Application.Interfaces;
using EduManagement.Core.Application.Services;
using EduManagement.Infrastructure.Caching;
using EduManagement.Infrastructure.Messaging;
using EduManagement.Infrastructure.Persistence;
using EduManagement.Infrastructure.Persistence.Repositories;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Serilog;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((context, configuration) =>
    configuration.ReadFrom.Configuration(context.Configuration).WriteTo.Console());

builder.Services.AddControllers(options =>
{
    options.Filters.Add<FluentValidationActionFilter>();
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<EduDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddSingleton<IConnectionMultiplexer>(_ =>
    ConnectionMultiplexer.Connect(builder.Configuration.GetConnectionString("Redis")!));

builder.Services.Configure<RabbitMqOptions>(builder.Configuration.GetSection(RabbitMqOptions.SectionName));
builder.Services.AddSingleton<RabbitMqConnectionProvider>();
builder.Services.AddSingleton<IEventBus, RabbitMqEventBus>();

builder.Services.AddValidatorsFromAssembly(typeof(EduManagement.Core.Application.Validators.CreateStudentDtoValidator).Assembly);

builder.Services.AddScoped<IStudentRepository, StudentRepository>();
builder.Services.AddScoped<ITeacherRepository, TeacherRepository>();
builder.Services.AddScoped<ISubjectRepository, SubjectRepository>();
builder.Services.AddScoped<ISchoolClassRepository, SchoolClassRepository>();
builder.Services.AddScoped<IEnrollmentRepository, EnrollmentRepository>();
builder.Services.AddScoped<IGradeRepository, GradeRepository>();
builder.Services.AddScoped<IGradeCacheService, RedisGradeCacheService>();
builder.Services.AddSingleton<GpaCalculationService>();
builder.Services.AddScoped<GradeService>();

builder.Services.AddHealthChecks()
    .AddCheck<RabbitMqHealthCheck>("rabbitmq");

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<EduDbContext>();
    db.Database.Migrate();
}

app.UseMiddleware<ExceptionHandlingMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.MapHealthChecks("/health");

app.Run();

public partial class Program;
