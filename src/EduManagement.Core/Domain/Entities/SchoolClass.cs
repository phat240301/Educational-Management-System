namespace EduManagement.Core.Domain.Entities;

// Maps to the `classes` table. Named SchoolClass because `class` is a reserved C# keyword.
public class SchoolClass
{
    public long Id { get; set; }
    public string ClassCode { get; set; } = string.Empty;
    public long SubjectId { get; set; }
    public long TeacherId { get; set; }
    public string AcademicYear { get; set; } = string.Empty;
    public short Semester { get; set; }
    public string? Room { get; set; }
    public short Capacity { get; set; } = 40;
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }

    public Subject Subject { get; set; } = null!;
    public Teacher Teacher { get; set; } = null!;
    public ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();
}
