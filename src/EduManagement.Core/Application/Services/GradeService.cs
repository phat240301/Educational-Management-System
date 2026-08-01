using EduManagement.Core.Application.DTOs;
using EduManagement.Core.Application.Events;
using EduManagement.Core.Application.Exceptions;
using EduManagement.Core.Application.Interfaces;
using EduManagement.Core.Domain.Entities;

namespace EduManagement.Core.Application.Services;

public class GradeService(
    IGradeRepository gradeRepository,
    IStudentRepository studentRepository,
    ISchoolClassRepository classRepository,
    IGradeCacheService cacheService,
    IEventBus eventBus,
    GpaCalculationService gpaCalculationService)
{
    public async Task<GradeResponseDto> CreateGradeAsync(CreateGradeDto dto, CancellationToken ct = default)
    {
        if (await studentRepository.GetByIdAsync(dto.StudentId, ct) is null)
        {
            throw new NotFoundException($"Student {dto.StudentId} does not exist.");
        }

        var schoolClass = await classRepository.GetByIdAsync(dto.ClassId, ct)
            ?? throw new NotFoundException($"Class {dto.ClassId} does not exist.");

        if (await gradeRepository.GetByStudentAndClassAsync(dto.StudentId, dto.ClassId, ct) is not null)
        {
            throw new ConflictException(
                $"A grade already exists for student {dto.StudentId} in class {dto.ClassId}. Use update instead.");
        }

        var grade = new Grade
        {
            StudentId = dto.StudentId,
            ClassId = dto.ClassId,
            SubjectId = schoolClass.SubjectId,
            AttendanceScore = dto.AttendanceScore,
            MidtermScore = dto.MidtermScore,
            FinalScore = dto.FinalScore,
            FinalGpa = gpaCalculationService.Calculate(dto.AttendanceScore, dto.MidtermScore, dto.FinalScore)
        };

        await gradeRepository.AddAsync(grade, ct);
        await gradeRepository.SaveChangesAsync(ct);
        await cacheService.InvalidateStudentReportAsync(dto.StudentId, ct);

        var created = await gradeRepository.GetByIdWithDetailsAsync(grade.Id, ct);
        await PublishGradeUpdatedEventAsync(created!, ct);
        return ToResponseDto(created!);
    }

    public async Task<GradeResponseDto> UpdateGradeAsync(long id, UpdateGradeDto dto, CancellationToken ct = default)
    {
        var grade = await gradeRepository.GetByIdAsync(id, ct)
            ?? throw new NotFoundException($"Grade {id} does not exist.");

        grade.AttendanceScore = dto.AttendanceScore;
        grade.MidtermScore = dto.MidtermScore;
        grade.FinalScore = dto.FinalScore;
        grade.FinalGpa = gpaCalculationService.Calculate(dto.AttendanceScore, dto.MidtermScore, dto.FinalScore);

        gradeRepository.Update(grade);
        await gradeRepository.SaveChangesAsync(ct);
        await cacheService.InvalidateStudentReportAsync(grade.StudentId, ct);

        var updated = await gradeRepository.GetByIdWithDetailsAsync(id, ct);
        await PublishGradeUpdatedEventAsync(updated!, ct);
        return ToResponseDto(updated!);
    }

    public async Task<GradeResponseDto?> GetByIdAsync(long id, CancellationToken ct = default)
    {
        var grade = await gradeRepository.GetByIdWithDetailsAsync(id, ct);
        return grade is null ? null : ToResponseDto(grade);
    }

    public async Task<StudentReportDto> GetStudentReportAsync(long studentId, CancellationToken ct = default)
    {
        var cached = await cacheService.GetStudentReportAsync(studentId, ct);
        if (cached is not null)
        {
            return cached;
        }

        var student = await studentRepository.GetByIdAsync(studentId, ct)
            ?? throw new NotFoundException($"Student {studentId} does not exist.");

        var grades = await gradeRepository.GetByStudentIdAsync(studentId, ct);
        var gradeDtos = grades.Select(ToResponseDto).ToList();
        var completedGpas = gradeDtos.Where(g => g.FinalGpa.HasValue).Select(g => g.FinalGpa!.Value).ToList();
        var overallGpa = completedGpas.Count > 0
            ? Math.Round(completedGpas.Average(), 2, MidpointRounding.AwayFromZero)
            : (decimal?)null;

        var report = new StudentReportDto(studentId, student.FullName, gradeDtos, overallGpa);
        await cacheService.SetStudentReportAsync(studentId, report, ct);

        return report;
    }

    private Task PublishGradeUpdatedEventAsync(Grade grade, CancellationToken ct) => eventBus.PublishAsync(
        new GradeUpdatedEvent(
            grade.Id, grade.StudentId, grade.Student.FullName, grade.Student.ParentEmail,
            grade.ClassId, grade.Class.ClassCode,
            grade.AttendanceScore, grade.MidtermScore, grade.FinalScore, grade.FinalGpa,
            DateTimeOffset.UtcNow),
        ct);

    private static GradeResponseDto ToResponseDto(Grade g) => new(
        g.Id, g.StudentId, g.Student.FullName, g.ClassId, g.Class.ClassCode,
        g.SubjectId, g.Subject.SubjectName, g.AttendanceScore, g.MidtermScore, g.FinalScore, g.FinalGpa,
        g.CreatedAt, g.UpdatedAt);
}
