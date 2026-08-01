using EduManagement.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Testcontainers.PostgreSql;

namespace EduManagement.IntegrationTests;

public class PostgresFixture : IAsyncLifetime
{
    private readonly PostgreSqlContainer _container = new PostgreSqlBuilder("postgres:16-alpine")
        .WithDatabase("edu_management_test")
        .WithUsername("edu_admin")
        .WithPassword("test_password")
        .Build();

    public async Task InitializeAsync()
    {
        await _container.StartAsync();

        await using var context = CreateContext();
        await context.Database.MigrateAsync();
    }

    public async Task DisposeAsync() => await _container.DisposeAsync();

    public EduDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<EduDbContext>()
            .UseNpgsql(_container.GetConnectionString())
            .Options;

        return new EduDbContext(options);
    }
}
