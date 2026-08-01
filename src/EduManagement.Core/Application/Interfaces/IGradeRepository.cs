using EduManagement.Core.Domain.Entities;

namespace EduManagement.Core.Application.Interfaces;

public interface IGradeRepository : IRepository<Grade>
{
    Task<Grade?> GetByStudentAndClassAsync(long studentId, long classId, CancellationToken ct = default);
    Task<Grade?> GetByIdWithDetailsAsync(long id, CancellationToken ct = default);
    Task<IReadOnlyList<Grade>> GetByStudentIdAsync(long studentId, CancellationToken ct = default);
}
