using EduManagement.Core.Application.Events;

namespace EduManagement.Worker.Services;

public interface IEmailNotificationService
{
    Task SendGradeUpdatedNotificationAsync(GradeUpdatedEvent gradeEvent, CancellationToken ct = default);
}
