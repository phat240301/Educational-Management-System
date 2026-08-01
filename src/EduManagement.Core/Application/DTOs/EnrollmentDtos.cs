using EduManagement.Core.Domain.Enums;

namespace EduManagement.Core.Application.DTOs;

public record CreateEnrollmentDto(
    long StudentId,
    long ClassId);

public record UpdateEnrollmentStatusDto(
    EnrollmentStatus Status);

public record EnrollmentResponseDto(
    long Id,
    long StudentId,
    string StudentFullName,
    long ClassId,
    string ClassCode,
    DateTimeOffset EnrolledAt,
    EnrollmentStatus Status,
    DateTimeOffset CreatedAt,
    DateTimeOffset UpdatedAt);
