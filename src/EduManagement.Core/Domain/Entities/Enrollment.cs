using EduManagement.Core.Domain.Enums;

namespace EduManagement.Core.Domain.Entities;

public class Enrollment
{
    public long Id { get; set; }
    public long StudentId { get; set; }
    public long ClassId { get; set; }
    public DateTimeOffset EnrolledAt { get; set; }
    public EnrollmentStatus Status { get; set; } = EnrollmentStatus.Active;
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }

    public Student Student { get; set; } = null!;
    public SchoolClass Class { get; set; } = null!;
}
