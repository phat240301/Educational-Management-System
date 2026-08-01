using EduManagement.Core.Application.Interfaces;
using EduManagement.Core.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace EduManagement.Infrastructure.Persistence.Repositories;

public class TeacherRepository(EduDbContext context) : RepositoryBase<Teacher>(context), ITeacherRepository
{
    public Task<bool> TeacherCodeExistsAsync(string teacherCode, CancellationToken ct = default) =>
        DbSet.AnyAsync(t => t.TeacherCode == teacherCode, ct);

    public Task<bool> EmailExistsAsync(string email, CancellationToken ct = default) =>
        DbSet.AnyAsync(t => t.Email == email, ct);
}
