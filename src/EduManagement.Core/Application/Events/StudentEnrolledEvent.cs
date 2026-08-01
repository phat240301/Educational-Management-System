namespace EduManagement.Core.Application.Events;

public record StudentEnrolledEvent(
    long EnrollmentId,
    long StudentId,
    string StudentFullName,
    long ClassId,
    string ClassCode,
    DateTimeOffset OccurredAt);
