namespace EduManagement.Core.Application.DTOs;

public record CreateSchoolClassDto(
    string ClassCode,
    long SubjectId,
    long TeacherId,
    string AcademicYear,
    short Semester,
    string? Room,
    short Capacity);

public record UpdateSchoolClassDto(
    long SubjectId,
    long TeacherId,
    string AcademicYear,
    short Semester,
    string? Room,
    short Capacity);

public record SchoolClassResponseDto(
    long Id,
    string ClassCode,
    long SubjectId,
    string SubjectName,
    long TeacherId,
    string TeacherName,
    string AcademicYear,
    short Semester,
    string? Room,
    short Capacity,
    DateTimeOffset CreatedAt,
    DateTimeOffset UpdatedAt);
