using EduManagement.Core.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EduManagement.Infrastructure.Persistence.Configurations;

public class SchoolClassConfiguration : IEntityTypeConfiguration<SchoolClass>
{
    public void Configure(EntityTypeBuilder<SchoolClass> builder)
    {
        builder.ToTable("classes");

        builder.HasKey(c => c.Id);
        builder.Property(c => c.Id).HasColumnName("id").UseIdentityAlwaysColumn();

        builder.Property(c => c.ClassCode).HasColumnName("class_code").HasMaxLength(30).IsRequired();
        builder.Property(c => c.SubjectId).HasColumnName("subject_id").IsRequired();
        builder.Property(c => c.TeacherId).HasColumnName("teacher_id").IsRequired();
        builder.Property(c => c.AcademicYear).HasColumnName("academic_year").HasMaxLength(9).IsRequired();
        builder.Property(c => c.Semester).HasColumnName("semester").IsRequired();
        builder.Property(c => c.Room).HasColumnName("room").HasMaxLength(30);
        builder.Property(c => c.Capacity).HasColumnName("capacity").HasDefaultValue((short)40);
        builder.Property(c => c.CreatedAt).HasColumnName("created_at").HasDefaultValueSql("now()");
        builder.Property(c => c.UpdatedAt).HasColumnName("updated_at").HasDefaultValueSql("now()");

        builder.HasIndex(c => c.ClassCode).IsUnique();
        builder.HasIndex(c => c.SubjectId).HasDatabaseName("idx_classes_subject_id");
        builder.HasIndex(c => c.TeacherId).HasDatabaseName("idx_classes_teacher_id");
        builder.HasIndex(c => new { c.AcademicYear, c.Semester }).HasDatabaseName("idx_classes_year_semester");

        builder.HasOne(c => c.Subject)
            .WithMany(s => s.Classes)
            .HasForeignKey(c => c.SubjectId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(c => c.Teacher)
            .WithMany(t => t.Classes)
            .HasForeignKey(c => c.TeacherId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
