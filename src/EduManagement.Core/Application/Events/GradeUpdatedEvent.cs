namespace EduManagement.Core.Application.Events;

public record GradeUpdatedEvent(
    long GradeId,
    long StudentId,
    string StudentFullName,
    string ParentEmail,
    long ClassId,
    string ClassCode,
    decimal? AttendanceScore,
    decimal? MidtermScore,
    decimal? FinalScore,
    decimal? FinalGpa,
    DateTimeOffset OccurredAt);
