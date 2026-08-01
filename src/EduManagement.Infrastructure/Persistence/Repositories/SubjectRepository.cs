using EduManagement.Core.Application.Interfaces;
using EduManagement.Core.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace EduManagement.Infrastructure.Persistence.Repositories;

public class SubjectRepository(EduDbContext context) : RepositoryBase<Subject>(context), ISubjectRepository
{
    public Task<bool> SubjectCodeExistsAsync(string subjectCode, CancellationToken ct = default) =>
        DbSet.AnyAsync(s => s.SubjectCode == subjectCode, ct);
}
