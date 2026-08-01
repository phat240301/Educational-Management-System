using EduManagement.Core.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EduManagement.Infrastructure.Persistence.Configurations;

public class SubjectConfiguration : IEntityTypeConfiguration<Subject>
{
    public void Configure(EntityTypeBuilder<Subject> builder)
    {
        builder.ToTable("subjects");

        builder.HasKey(s => s.Id);
        builder.Property(s => s.Id).HasColumnName("id").UseIdentityAlwaysColumn();

        builder.Property(s => s.SubjectCode).HasColumnName("subject_code").HasMaxLength(20).IsRequired();
        builder.Property(s => s.SubjectName).HasColumnName("subject_name").HasMaxLength(150).IsRequired();
        builder.Property(s => s.CreditHours).HasColumnName("credit_hours").HasDefaultValue((short)3);
        builder.Property(s => s.CreatedAt).HasColumnName("created_at").HasDefaultValueSql("now()");
        builder.Property(s => s.UpdatedAt).HasColumnName("updated_at").HasDefaultValueSql("now()");

        builder.HasIndex(s => s.SubjectCode).IsUnique();
    }
}
