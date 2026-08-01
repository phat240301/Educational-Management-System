using EduManagement.Core.Application.DTOs;
using EduManagement.Core.Application.Events;
using EduManagement.Core.Application.Exceptions;
using EduManagement.Core.Application.Interfaces;
using EduManagement.Core.Application.Services;
using EduManagement.Core.Domain.Entities;
using EduManagement.Infrastructure.Persistence;
using EduManagement.Infrastructure.Persistence.Repositories;
using FluentAssertions;
using Moq;

namespace EduManagement.IntegrationTests;

public class GradeServiceIntegrationTests(PostgresFixture fixture) : IClassFixture<PostgresFixture>, IAsyncLifetime
{
    private EduDbContext _context = null!;
    private GradeService _sut = null!;
    private readonly Mock<IGradeCacheService> _cacheService = new();
    private readonly Mock<IEventBus> _eventBus = new();

    private long _studentId;
    private long _classId;

    public async Task InitializeAsync()
    {
        _context = fixture.CreateContext();

        var teacher = new Teacher
        {
            TeacherCode = $"T-{Guid.NewGuid():N}"[..10],
            FullName = "Test Teacher",
            Email = $"{Guid.NewGuid():N}@example.com"
        };
        var subject = new Subject
        {
            SubjectCode = $"S-{Guid.NewGuid():N}"[..10],
            SubjectName = "Test Subject"
        };
        _context.Teachers.Add(teacher);
        _context.Subjects.Add(subject);
        await _context.SaveChangesAsync();

        var schoolClass = new SchoolClass
        {
            ClassCode = $"C-{Guid.NewGuid():N}"[..10],
            SubjectId = subject.Id,
            TeacherId = teacher.Id,
            AcademicYear = "2026",
            Semester = 1
        };
        var student = new Student
        {
            StudentCode = $"ST-{Guid.NewGuid():N}"[..10],
            FullName = "Test Student",
            ParentEmail = "parent@example.com"
        };
        _context.Classes.Add(schoolClass);
        _context.Students.Add(student);
        await _context.SaveChangesAsync();

        _studentId = student.Id;
        _classId = schoolClass.Id;

        var gradeRepository = new GradeRepository(_context);
        var studentRepository = new StudentRepository(_context);
        var classRepository = new SchoolClassRepository(_context);
        var gpaCalculationService = new GpaCalculationService();

        _sut = new GradeService(
            gradeRepository,
            studentRepository,
            classRepository,
            _cacheService.Object,
            _eventBus.Object,
            gpaCalculationService);
    }

    public async Task DisposeAsync() => await _context.DisposeAsync();

    [Fact]
    public async Task CreateGradeAsync_ValidData_PersistsGradeWithCalculatedGpa()
    {
        var dto = new CreateGradeDto(_studentId, _classId, 90, 80, 85);

        var result = await _sut.CreateGradeAsync(dto);

        result.FinalGpa.Should().Be(84.00m);
        result.StudentId.Should().Be(_studentId);
        result.ClassId.Should().Be(_classId);

        var persisted = await _sut.GetByIdAsync(result.Id);
        persisted.Should().NotBeNull();
        persisted!.FinalGpa.Should().Be(84.00m);
    }

    [Fact]
    public async Task CreateGradeAsync_DuplicateStudentAndClass_ThrowsConflictException()
    {
        var dto = new CreateGradeDto(_studentId, _classId, 90, 80, 85);
        await _sut.CreateGradeAsync(dto);

        var act = () => _sut.CreateGradeAsync(dto);

        await act.Should().ThrowAsync<ConflictException>();
    }

    [Fact]
    public async Task CreateGradeAsync_StudentDoesNotExist_ThrowsNotFoundException()
    {
        var dto = new CreateGradeDto(StudentId: 999_999, _classId, 90, 80, 85);

        var act = () => _sut.CreateGradeAsync(dto);

        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task CreateGradeAsync_PartialScores_PersistsWithNullGpa()
    {
        var dto = new CreateGradeDto(_studentId, _classId, 90, 80, null);

        var result = await _sut.CreateGradeAsync(dto);

        result.FinalGpa.Should().BeNull();
    }

    [Fact]
    public async Task UpdateGradeAsync_ExistingGrade_RecalculatesGpaAndInvalidatesCache()
    {
        var created = await _sut.CreateGradeAsync(new CreateGradeDto(_studentId, _classId, 90, 80, 85));

        var updated = await _sut.UpdateGradeAsync(created.Id, new UpdateGradeDto(100, 100, 100));

        updated.FinalGpa.Should().Be(100.00m);
        _cacheService.Verify(
            c => c.InvalidateStudentReportAsync(_studentId, It.IsAny<CancellationToken>()),
            Times.Exactly(2));
    }

    [Fact]
    public async Task CreateGradeAsync_PublishesGradeUpdatedEvent()
    {
        await _sut.CreateGradeAsync(new CreateGradeDto(_studentId, _classId, 90, 80, 85));

        _eventBus.Verify(
            e => e.PublishAsync(It.IsAny<GradeUpdatedEvent>(), It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task GetStudentReportAsync_NoCacheEntry_ComputesReportAndCachesIt()
    {
        await _sut.CreateGradeAsync(new CreateGradeDto(_studentId, _classId, 90, 80, 85));
        _cacheService
            .Setup(c => c.GetStudentReportAsync(_studentId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((StudentReportDto?)null);

        var report = await _sut.GetStudentReportAsync(_studentId);

        report.StudentId.Should().Be(_studentId);
        report.OverallGpa.Should().Be(84.00m);
        report.Grades.Should().HaveCount(1);
        _cacheService.Verify(
            c => c.SetStudentReportAsync(_studentId, It.IsAny<StudentReportDto>(), It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task GetStudentReportAsync_CacheHit_ReturnsCachedReportWithoutRecomputing()
    {
        var cached = new StudentReportDto(_studentId, "Cached Student", [], 42.00m);
        _cacheService
            .Setup(c => c.GetStudentReportAsync(_studentId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(cached);

        var report = await _sut.GetStudentReportAsync(_studentId);

        report.Should().BeEquivalentTo(cached);
        _cacheService.Verify(
            c => c.SetStudentReportAsync(It.IsAny<long>(), It.IsAny<StudentReportDto>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }
}
