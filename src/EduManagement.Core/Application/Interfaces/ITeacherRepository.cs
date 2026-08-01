using EduManagement.Core.Domain.Entities;

namespace EduManagement.Core.Application.Interfaces;

public interface ITeacherRepository : IRepository<Teacher>
{
    Task<bool> TeacherCodeExistsAsync(string teacherCode, CancellationToken ct = default);
    Task<bool> EmailExistsAsync(string email, CancellationToken ct = default);
}
