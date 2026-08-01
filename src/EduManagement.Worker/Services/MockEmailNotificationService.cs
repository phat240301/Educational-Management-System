using EduManagement.Core.Application.Events;
using Microsoft.Extensions.Logging;

namespace EduManagement.Worker.Services;

// Simulates sending an email via SMTP. Swap for MailKit/SendGrid/SES when a real
// provider is wired up — the consumer only depends on IEmailNotificationService.
public class MockEmailNotificationService(ILogger<MockEmailNotificationService> logger) : IEmailNotificationService
{
    public async Task SendGradeUpdatedNotificationAsync(GradeUpdatedEvent gradeEvent, CancellationToken ct = default)
    {
        await Task.Delay(200, ct);

        logger.LogInformation(
            "[MOCK EMAIL] To: {ParentEmail} | Subject: Grade update for {StudentFullName} | " +
            "Class: {ClassCode} | Attendance: {AttendanceScore} Midterm: {MidtermScore} Final: {FinalScore} | Final GPA: {FinalGpa}",
            gradeEvent.ParentEmail, gradeEvent.StudentFullName, gradeEvent.ClassCode,
            gradeEvent.AttendanceScore, gradeEvent.MidtermScore, gradeEvent.FinalScore, gradeEvent.FinalGpa);
    }
}
