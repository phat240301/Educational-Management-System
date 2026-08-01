using EduManagement.Core.Application.Interfaces;
using EduManagement.Core.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace EduManagement.Infrastructure.Persistence.Repositories;

public class SchoolClassRepository(EduDbContext context)
    : RepositoryBase<SchoolClass>(context), ISchoolClassRepository
{
    public Task<bool> ClassCodeExistsAsync(string classCode, CancellationToken ct = default) =>
        DbSet.AnyAsync(c => c.ClassCode == classCode, ct);

    public Task<bool> ExistsAsync(long id, CancellationToken ct = default) =>
        DbSet.AnyAsync(c => c.Id == id, ct);

    public Task<SchoolClass?> GetByIdWithDetailsAsync(long id, CancellationToken ct = default) =>
        DbSet.Include(c => c.Subject)
            .Include(c => c.Teacher)
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == id, ct);

    public async Task<IReadOnlyList<SchoolClass>> GetAllWithDetailsAsync(CancellationToken ct = default) =>
        await DbSet.Include(c => c.Subject)
            .Include(c => c.Teacher)
            .AsNoTracking()
            .ToListAsync(ct);
}
