namespace EduManagement.Core.Application.DTOs;

public record CreateSubjectDto(
    string SubjectCode,
    string SubjectName,
    short CreditHours);

public record UpdateSubjectDto(
    string SubjectName,
    short CreditHours);

public record SubjectResponseDto(
    long Id,
    string SubjectCode,
    string SubjectName,
    short CreditHours,
    DateTimeOffset CreatedAt,
    DateTimeOffset UpdatedAt);
