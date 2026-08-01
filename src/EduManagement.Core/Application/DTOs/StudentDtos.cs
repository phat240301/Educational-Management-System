namespace EduManagement.Core.Application.DTOs;

public record CreateStudentDto(
    string StudentCode,
    string FullName,
    DateOnly DateOfBirth,
    string ParentEmail,
    string? ParentPhone);

public record UpdateStudentDto(
    string FullName,
    DateOnly DateOfBirth,
    string ParentEmail,
    string? ParentPhone,
    bool IsActive);

public record StudentResponseDto(
    long Id,
    string StudentCode,
    string FullName,
    DateOnly DateOfBirth,
    string ParentEmail,
    string? ParentPhone,
    bool IsActive,
    DateTimeOffset CreatedAt,
    DateTimeOffset UpdatedAt);
