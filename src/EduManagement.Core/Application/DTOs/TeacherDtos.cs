namespace EduManagement.Core.Application.DTOs;

public record CreateTeacherDto(
    string TeacherCode,
    string FullName,
    string Email);

public record UpdateTeacherDto(
    string FullName,
    string Email,
    bool IsActive);

public record TeacherResponseDto(
    long Id,
    string TeacherCode,
    string FullName,
    string Email,
    bool IsActive,
    DateTimeOffset CreatedAt,
    DateTimeOffset UpdatedAt);
