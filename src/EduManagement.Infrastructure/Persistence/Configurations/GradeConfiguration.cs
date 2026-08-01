using EduManagement.Core.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EduManagement.Infrastructure.Persistence.Configurations;

public class GradeConfiguration : IEntityTypeConfiguration<Grade>
{
    public void Configure(EntityTypeBuilder<Grade> builder)
    {
        builder.ToTable("grades", t => t.HasCheckConstraint(
            "ck_grades_score_range",
            "(attendance_score IS NULL OR attendance_score BETWEEN 0 AND 100) " +
            "AND (midterm_score IS NULL OR midterm_score BETWEEN 0 AND 100) " +
            "AND (final_score IS NULL OR final_score BETWEEN 0 AND 100)"));

        builder.HasKey(g => g.Id);
        builder.Property(g => g.Id).HasColumnName("id").UseIdentityAlwaysColumn();

        builder.Property(g => g.StudentId).HasColumnName("student_id").IsRequired();
        builder.Property(g => g.ClassId).HasColumnName("class_id").IsRequired();
        builder.Property(g => g.SubjectId).HasColumnName("subject_id").IsRequired();
        builder.Property(g => g.AttendanceScore).HasColumnName("attendance_score").HasColumnType("numeric(5,2)");
        builder.Property(g => g.MidtermScore).HasColumnName("midterm_score").HasColumnType("numeric(5,2)");
        builder.Property(g => g.FinalScore).HasColumnName("final_score").HasColumnType("numeric(5,2)");
        builder.Property(g => g.FinalGpa).HasColumnName("final_gpa").HasColumnType("numeric(5,2)");
        builder.Property(g => g.CreatedAt).HasColumnName("created_at").HasDefaultValueSql("now()");
        builder.Property(g => g.UpdatedAt).HasColumnName("updated_at").HasDefaultValueSql("now()");

        builder.HasIndex(g => new { g.StudentId, g.ClassId }).IsUnique().HasDatabaseName("uq_grade_student_class");
        builder.HasIndex(g => g.StudentId).HasDatabaseName("idx_grades_student_id");
        builder.HasIndex(g => g.ClassId).HasDatabaseName("idx_grades_class_id");

        builder.HasOne(g => g.Student)
            .WithMany()
            .HasForeignKey(g => g.StudentId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(g => g.Class)
            .WithMany()
            .HasForeignKey(g => g.ClassId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(g => g.Subject)
            .WithMany()
            .HasForeignKey(g => g.SubjectId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
