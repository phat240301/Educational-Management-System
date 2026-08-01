namespace EduManagement.Core.Application.DTOs;

public record CreateGradeDto(
    long StudentId,
    long ClassId,
    decimal? AttendanceScore,
    decimal? MidtermScore,
    decimal? FinalScore);

public record UpdateGradeDto(
    decimal? AttendanceScore,
    decimal? MidtermScore,
    decimal? FinalScore);

public record GradeResponseDto(
    long Id,
    long StudentId,
    string StudentFullName,
    long ClassId,
    string ClassCode,
    long SubjectId,
    string SubjectName,
    decimal? AttendanceScore,
    decimal? MidtermScore,
    decimal? FinalScore,
    decimal? FinalGpa,
    DateTimeOffset CreatedAt,
    DateTimeOffset UpdatedAt);

public record StudentReportDto(
    long StudentId,
    string StudentFullName,
    IReadOnlyList<GradeResponseDto> Grades,
    decimal? OverallGpa);
