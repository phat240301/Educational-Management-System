using EduManagement.Core.Application.Interfaces;
using EduManagement.Core.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace EduManagement.Infrastructure.Persistence.Repositories;

public class GradeRepository(EduDbContext context) : RepositoryBase<Grade>(context), IGradeRepository
{
    public Task<Grade?> GetByStudentAndClassAsync(long studentId, long classId, CancellationToken ct = default) =>
        DbSet.FirstOrDefaultAsync(g => g.StudentId == studentId && g.ClassId == classId, ct);

    public Task<Grade?> GetByIdWithDetailsAsync(long id, CancellationToken ct = default) =>
        DbSet.Include(g => g.Student)
            .Include(g => g.Class)
            .Include(g => g.Subject)
            .AsNoTracking()
            .FirstOrDefaultAsync(g => g.Id == id, ct);

    public async Task<IReadOnlyList<Grade>> GetByStudentIdAsync(long studentId, CancellationToken ct = default) =>
        await DbSet.Include(g => g.Student)
            .Include(g => g.Class)
            .Include(g => g.Subject)
            .AsNoTracking()
            .Where(g => g.StudentId == studentId)
            .ToListAsync(ct);
}
