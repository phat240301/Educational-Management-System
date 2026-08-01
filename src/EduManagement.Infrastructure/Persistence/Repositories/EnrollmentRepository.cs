using EduManagement.Core.Application.Interfaces;
using EduManagement.Core.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace EduManagement.Infrastructure.Persistence.Repositories;

public class EnrollmentRepository(EduDbContext context)
    : RepositoryBase<Enrollment>(context), IEnrollmentRepository
{
    public Task<bool> AlreadyEnrolledAsync(long studentId, long classId, CancellationToken ct = default) =>
        DbSet.AnyAsync(e => e.StudentId == studentId && e.ClassId == classId, ct);

    public Task<Enrollment?> GetByIdWithDetailsAsync(long id, CancellationToken ct = default) =>
        DbSet.Include(e => e.Student)
            .Include(e => e.Class)
            .AsNoTracking()
            .FirstOrDefaultAsync(e => e.Id == id, ct);

    public async Task<IReadOnlyList<Enrollment>> GetAllWithDetailsAsync(CancellationToken ct = default) =>
        await DbSet.Include(e => e.Student)
            .Include(e => e.Class)
            .AsNoTracking()
            .ToListAsync(ct);

    public async Task<IReadOnlyList<Enrollment>> GetByStudentIdAsync(long studentId, CancellationToken ct = default) =>
        await DbSet.Include(e => e.Class)
            .AsNoTracking()
            .Where(e => e.StudentId == studentId)
            .ToListAsync(ct);
}
