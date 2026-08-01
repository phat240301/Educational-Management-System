using EduManagement.Core.Application.Interfaces;
using EduManagement.Core.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace EduManagement.Infrastructure.Persistence.Repositories;

public class StudentRepository(EduDbContext context) : RepositoryBase<Student>(context), IStudentRepository
{
    public Task<bool> StudentCodeExistsAsync(string studentCode, CancellationToken ct = default) =>
        DbSet.AnyAsync(s => s.StudentCode == studentCode, ct);
}
