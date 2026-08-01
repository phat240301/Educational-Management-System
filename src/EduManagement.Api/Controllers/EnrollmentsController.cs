using EduManagement.Core.Application.DTOs;
using EduManagement.Core.Application.Events;
using EduManagement.Core.Application.Interfaces;
using EduManagement.Core.Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace EduManagement.Api.Controllers;

[ApiController]
[Route("api/enrollments")]
public class EnrollmentsController(
    IEnrollmentRepository enrollmentRepository,
    IStudentRepository studentRepository,
    ISchoolClassRepository classRepository,
    IEventBus eventBus) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<EnrollmentResponseDto>>> GetAll(CancellationToken ct)
    {
        var enrollments = await enrollmentRepository.GetAllWithDetailsAsync(ct);
        return Ok(enrollments.Select(ToResponseDto));
    }

    [HttpGet("{id:long}")]
    public async Task<ActionResult<EnrollmentResponseDto>> GetById(long id, CancellationToken ct)
    {
        var enrollment = await enrollmentRepository.GetByIdWithDetailsAsync(id, ct);
        return enrollment is null ? NotFound() : Ok(ToResponseDto(enrollment));
    }

    [HttpGet("~/api/students/{studentId:long}/enrollments")]
    public async Task<ActionResult<IReadOnlyList<EnrollmentResponseDto>>> GetByStudent(long studentId, CancellationToken ct)
    {
        if (await studentRepository.GetByIdAsync(studentId, ct) is null)
        {
            return NotFound($"Student {studentId} does not exist.");
        }

        var enrollments = await enrollmentRepository.GetByStudentIdAsync(studentId, ct);
        return Ok(enrollments.Select(e => new EnrollmentResponseDto(
            e.Id, e.StudentId, string.Empty, e.ClassId, e.Class.ClassCode,
            e.EnrolledAt, e.Status, e.CreatedAt, e.UpdatedAt)));
    }

    [HttpPost]
    public async Task<ActionResult<EnrollmentResponseDto>> Create(CreateEnrollmentDto dto, CancellationToken ct)
    {
        if (await studentRepository.GetByIdAsync(dto.StudentId, ct) is null)
        {
            return BadRequest($"Student {dto.StudentId} does not exist.");
        }

        if (!await classRepository.ExistsAsync(dto.ClassId, ct))
        {
            return BadRequest($"Class {dto.ClassId} does not exist.");
        }

        if (await enrollmentRepository.AlreadyEnrolledAsync(dto.StudentId, dto.ClassId, ct))
        {
            return Conflict($"Student {dto.StudentId} is already enrolled in class {dto.ClassId}.");
        }

        var enrollment = new Enrollment
        {
            StudentId = dto.StudentId,
            ClassId = dto.ClassId,
            EnrolledAt = DateTimeOffset.UtcNow
        };

        await enrollmentRepository.AddAsync(enrollment, ct);
        await enrollmentRepository.SaveChangesAsync(ct);

        var created = await enrollmentRepository.GetByIdWithDetailsAsync(enrollment.Id, ct);

        await eventBus.PublishAsync(new StudentEnrolledEvent(
            created!.Id, created.StudentId, created.Student.FullName,
            created.ClassId, created.Class.ClassCode, DateTimeOffset.UtcNow), ct);

        return CreatedAtAction(nameof(GetById), new { id = enrollment.Id }, ToResponseDto(created));
    }

    [HttpPut("{id:long}/status")]
    public async Task<ActionResult<EnrollmentResponseDto>> UpdateStatus(long id, UpdateEnrollmentStatusDto dto, CancellationToken ct)
    {
        var enrollment = await enrollmentRepository.GetByIdAsync(id, ct);
        if (enrollment is null)
        {
            return NotFound();
        }

        enrollment.Status = dto.Status;

        enrollmentRepository.Update(enrollment);
        await enrollmentRepository.SaveChangesAsync(ct);

        var updated = await enrollmentRepository.GetByIdWithDetailsAsync(id, ct);
        return Ok(ToResponseDto(updated!));
    }

    [HttpDelete("{id:long}")]
    public async Task<IActionResult> Delete(long id, CancellationToken ct)
    {
        var enrollment = await enrollmentRepository.GetByIdAsync(id, ct);
        if (enrollment is null)
        {
            return NotFound();
        }

        enrollmentRepository.Remove(enrollment);
        await enrollmentRepository.SaveChangesAsync(ct);

        return NoContent();
    }

    private static EnrollmentResponseDto ToResponseDto(Enrollment e) => new(
        e.Id, e.StudentId, e.Student.FullName, e.ClassId, e.Class.ClassCode,
        e.EnrolledAt, e.Status, e.CreatedAt, e.UpdatedAt);
}
