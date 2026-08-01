using EduManagement.Core.Application.DTOs;

namespace EduManagement.Core.Application.Interfaces;

public interface IGradeCacheService
{
    Task<StudentReportDto?> GetStudentReportAsync(long studentId, CancellationToken ct = default);
    Task SetStudentReportAsync(long studentId, StudentReportDto report, CancellationToken ct = default);
    Task InvalidateStudentReportAsync(long studentId, CancellationToken ct = default);
}
