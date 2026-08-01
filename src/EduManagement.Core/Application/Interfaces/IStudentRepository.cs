using EduManagement.Core.Domain.Entities;

namespace EduManagement.Core.Application.Interfaces;

public interface IStudentRepository : IRepository<Student>
{
    Task<bool> StudentCodeExistsAsync(string studentCode, CancellationToken ct = default);
}
