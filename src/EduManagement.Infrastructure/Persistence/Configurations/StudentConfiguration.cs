using EduManagement.Core.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EduManagement.Infrastructure.Persistence.Configurations;

public class StudentConfiguration : IEntityTypeConfiguration<Student>
{
    public void Configure(EntityTypeBuilder<Student> builder)
    {
        builder.ToTable("students");

        builder.HasKey(s => s.Id);
        builder.Property(s => s.Id).HasColumnName("id").UseIdentityAlwaysColumn();

        builder.Property(s => s.StudentCode).HasColumnName("student_code").HasMaxLength(20).IsRequired();
        builder.Property(s => s.FullName).HasColumnName("full_name").HasMaxLength(150).IsRequired();
        builder.Property(s => s.DateOfBirth).HasColumnName("date_of_birth").IsRequired();
        builder.Property(s => s.ParentEmail).HasColumnName("parent_email").HasMaxLength(200).IsRequired();
        builder.Property(s => s.ParentPhone).HasColumnName("parent_phone").HasMaxLength(20);
        builder.Property(s => s.IsActive).HasColumnName("is_active").HasDefaultValue(true);
        builder.Property(s => s.CreatedAt).HasColumnName("created_at").HasDefaultValueSql("now()");
        builder.Property(s => s.UpdatedAt).HasColumnName("updated_at").HasDefaultValueSql("now()");

        builder.HasIndex(s => s.StudentCode).IsUnique();
        builder.HasIndex(s => s.IsActive).HasDatabaseName("idx_students_active");
    }
}
