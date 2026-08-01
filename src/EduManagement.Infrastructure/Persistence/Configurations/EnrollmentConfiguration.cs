using EduManagement.Core.Domain.Entities;
using EduManagement.Core.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EduManagement.Infrastructure.Persistence.Configurations;

public class EnrollmentConfiguration : IEntityTypeConfiguration<Enrollment>
{
    public void Configure(EntityTypeBuilder<Enrollment> builder)
    {
        builder.ToTable("enrollments");

        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id).HasColumnName("id").UseIdentityAlwaysColumn();

        builder.Property(e => e.StudentId).HasColumnName("student_id").IsRequired();
        builder.Property(e => e.ClassId).HasColumnName("class_id").IsRequired();
        builder.Property(e => e.EnrolledAt).HasColumnName("enrolled_at").HasDefaultValueSql("now()");
        builder.Property(e => e.Status)
            .HasColumnName("status")
            .HasMaxLength(20)
            .HasConversion(
                v => v.ToString().ToUpperInvariant(),
                v => Enum.Parse<EnrollmentStatus>(v, ignoreCase: true))
            .HasDefaultValue(EnrollmentStatus.Active);
        builder.Property(e => e.CreatedAt).HasColumnName("created_at").HasDefaultValueSql("now()");
        builder.Property(e => e.UpdatedAt).HasColumnName("updated_at").HasDefaultValueSql("now()");

        builder.HasIndex(e => new { e.StudentId, e.ClassId }).IsUnique().HasDatabaseName("uq_enrollment");
        builder.HasIndex(e => e.StudentId).HasDatabaseName("idx_enrollments_student_id");
        builder.HasIndex(e => e.ClassId).HasDatabaseName("idx_enrollments_class_id");

        builder.HasOne(e => e.Student)
            .WithMany(s => s.Enrollments)
            .HasForeignKey(e => e.StudentId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(e => e.Class)
            .WithMany(c => c.Enrollments)
            .HasForeignKey(e => e.ClassId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
