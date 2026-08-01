using EduManagement.Core.Domain.Entities;

namespace EduManagement.Core.Application.Interfaces;

public interface ISchoolClassRepository : IRepository<SchoolClass>
{
    Task<bool> ClassCodeExistsAsync(string classCode, CancellationToken ct = default);
    Task<SchoolClass?> GetByIdWithDetailsAsync(long id, CancellationToken ct = default);
    Task<IReadOnlyList<SchoolClass>> GetAllWithDetailsAsync(CancellationToken ct = default);
    Task<bool> ExistsAsync(long id, CancellationToken ct = default);
}
