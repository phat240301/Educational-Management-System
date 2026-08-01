using EduManagement.Core.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EduManagement.Infrastructure.Persistence.Configurations;

public class TeacherConfiguration : IEntityTypeConfiguration<Teacher>
{
    public void Configure(EntityTypeBuilder<Teacher> builder)
    {
        builder.ToTable("teachers");

        builder.HasKey(t => t.Id);
        builder.Property(t => t.Id).HasColumnName("id").UseIdentityAlwaysColumn();

        builder.Property(t => t.TeacherCode).HasColumnName("teacher_code").HasMaxLength(20).IsRequired();
        builder.Property(t => t.FullName).HasColumnName("full_name").HasMaxLength(150).IsRequired();
        builder.Property(t => t.Email).HasColumnName("email").HasMaxLength(200).IsRequired();
        builder.Property(t => t.IsActive).HasColumnName("is_active").HasDefaultValue(true);
        builder.Property(t => t.CreatedAt).HasColumnName("created_at").HasDefaultValueSql("now()");
        builder.Property(t => t.UpdatedAt).HasColumnName("updated_at").HasDefaultValueSql("now()");

        builder.HasIndex(t => t.TeacherCode).IsUnique();
        builder.HasIndex(t => t.Email).IsUnique();
    }
}
