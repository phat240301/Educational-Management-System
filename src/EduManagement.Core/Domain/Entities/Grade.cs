namespace EduManagement.Core.Domain.Entities;

public class Grade
{
    public long Id { get; set; }
    public long StudentId { get; set; }
    public long ClassId { get; set; }
    public long SubjectId { get; set; }
    public decimal? AttendanceScore { get; set; }
    public decimal? MidtermScore { get; set; }
    public decimal? FinalScore { get; set; }
    public decimal? FinalGpa { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }

    public Student Student { get; set; } = null!;
    public SchoolClass Class { get; set; } = null!;
    public Subject Subject { get; set; } = null!;
}
