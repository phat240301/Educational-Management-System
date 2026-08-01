using EduManagement.Core.Domain.Entities;

namespace EduManagement.Core.Application.Interfaces;

public interface ISubjectRepository : IRepository<Subject>
{
    Task<bool> SubjectCodeExistsAsync(string subjectCode, CancellationToken ct = default);
}
