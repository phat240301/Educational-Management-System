using System.Text.Json;
using EduManagement.Core.Application.DTOs;
using EduManagement.Core.Application.Interfaces;
using StackExchange.Redis;

namespace EduManagement.Infrastructure.Caching;

public class RedisGradeCacheService(IConnectionMultiplexer connectionMultiplexer) : IGradeCacheService
{
    private static readonly TimeSpan CacheTtl = TimeSpan.FromMinutes(10);

    private IDatabase Database => connectionMultiplexer.GetDatabase();

    private static string ReportKey(long studentId) => $"grade:student:{studentId}:report";

    public async Task<StudentReportDto?> GetStudentReportAsync(long studentId, CancellationToken ct = default)
    {
        var value = await Database.StringGetAsync(ReportKey(studentId));
        return value.IsNullOrEmpty ? null : JsonSerializer.Deserialize<StudentReportDto>((string)value!);
    }

    public async Task SetStudentReportAsync(long studentId, StudentReportDto report, CancellationToken ct = default)
    {
        var json = JsonSerializer.Serialize(report);
        await Database.StringSetAsync(ReportKey(studentId), json, CacheTtl);
    }

    public async Task InvalidateStudentReportAsync(long studentId, CancellationToken ct = default)
    {
        await Database.KeyDeleteAsync(ReportKey(studentId));
    }
}
