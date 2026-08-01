using EduManagement.Core.Domain.Entities;

namespace EduManagement.Core.Application.Interfaces;

public interface IEnrollmentRepository : IRepository<Enrollment>
{
    Task<bool> AlreadyEnrolledAsync(long studentId, long classId, CancellationToken ct = default);
    Task<Enrollment?> GetByIdWithDetailsAsync(long id, CancellationToken ct = default);
    Task<IReadOnlyList<Enrollment>> GetAllWithDetailsAsync(CancellationToken ct = default);
    Task<IReadOnlyList<Enrollment>> GetByStudentIdAsync(long studentId, CancellationToken ct = default);
}
