using EduManagement.Core.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace EduManagement.Infrastructure.Persistence;

public class EduDbContext(DbContextOptions<EduDbContext> options) : DbContext(options)
{
    public DbSet<Student> Students => Set<Student>();
    public DbSet<Teacher> Teachers => Set<Teacher>();
    public DbSet<Subject> Subjects => Set<Subject>();
    public DbSet<SchoolClass> Classes => Set<SchoolClass>();
    public DbSet<Enrollment> Enrollments => Set<Enrollment>();
    public DbSet<Grade> Grades => Set<Grade>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(EduDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var now = DateTimeOffset.UtcNow;

        foreach (var entry in ChangeTracker.Entries())
        {
            if (entry.Properties.All(p => p.Metadata.Name != nameof(Student.CreatedAt)))
            {
                continue;
            }

            if (entry.State == EntityState.Added)
            {
                entry.Property(nameof(Student.CreatedAt)).CurrentValue = now;
                entry.Property(nameof(Student.UpdatedAt)).CurrentValue = now;
            }
            else if (entry.State == EntityState.Modified)
            {
                entry.Property(nameof(Student.UpdatedAt)).CurrentValue = now;
            }
        }

        return base.SaveChangesAsync(cancellationToken);
    }
}
