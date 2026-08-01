using EduManagement.Core.Application.DTOs;
using EduManagement.Core.Application.Interfaces;
using EduManagement.Core.Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace EduManagement.Api.Controllers;

[ApiController]
[Route("api/classes")]
public class ClassesController(
    ISchoolClassRepository classRepository,
    ISubjectRepository subjectRepository,
    ITeacherRepository teacherRepository) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<SchoolClassResponseDto>>> GetAll(CancellationToken ct)
    {
        var classes = await classRepository.GetAllWithDetailsAsync(ct);
        return Ok(classes.Select(ToResponseDto));
    }

    [HttpGet("{id:long}")]
    public async Task<ActionResult<SchoolClassResponseDto>> GetById(long id, CancellationToken ct)
    {
        var schoolClass = await classRepository.GetByIdWithDetailsAsync(id, ct);
        return schoolClass is null ? NotFound() : Ok(ToResponseDto(schoolClass));
    }

    [HttpPost]
    public async Task<ActionResult<SchoolClassResponseDto>> Create(CreateSchoolClassDto dto, CancellationToken ct)
    {
        if (await classRepository.ClassCodeExistsAsync(dto.ClassCode, ct))
        {
            return Conflict($"Class code '{dto.ClassCode}' already exists.");
        }

        if (await subjectRepository.GetByIdAsync(dto.SubjectId, ct) is null)
        {
            return BadRequest($"Subject {dto.SubjectId} does not exist.");
        }

        if (await teacherRepository.GetByIdAsync(dto.TeacherId, ct) is null)
        {
            return BadRequest($"Teacher {dto.TeacherId} does not exist.");
        }

        var schoolClass = new SchoolClass
        {
            ClassCode = dto.ClassCode,
            SubjectId = dto.SubjectId,
            TeacherId = dto.TeacherId,
            AcademicYear = dto.AcademicYear,
            Semester = dto.Semester,
            Room = dto.Room,
            Capacity = dto.Capacity
        };

        await classRepository.AddAsync(schoolClass, ct);
        await classRepository.SaveChangesAsync(ct);

        var created = await classRepository.GetByIdWithDetailsAsync(schoolClass.Id, ct);
        return CreatedAtAction(nameof(GetById), new { id = schoolClass.Id }, ToResponseDto(created!));
    }

    [HttpPut("{id:long}")]
    public async Task<ActionResult<SchoolClassResponseDto>> Update(long id, UpdateSchoolClassDto dto, CancellationToken ct)
    {
        var schoolClass = await classRepository.GetByIdAsync(id, ct);
        if (schoolClass is null)
        {
            return NotFound();
        }

        if (await subjectRepository.GetByIdAsync(dto.SubjectId, ct) is null)
        {
            return BadRequest($"Subject {dto.SubjectId} does not exist.");
        }

        if (await teacherRepository.GetByIdAsync(dto.TeacherId, ct) is null)
        {
            return BadRequest($"Teacher {dto.TeacherId} does not exist.");
        }

        schoolClass.SubjectId = dto.SubjectId;
        schoolClass.TeacherId = dto.TeacherId;
        schoolClass.AcademicYear = dto.AcademicYear;
        schoolClass.Semester = dto.Semester;
        schoolClass.Room = dto.Room;
        schoolClass.Capacity = dto.Capacity;

        classRepository.Update(schoolClass);
        await classRepository.SaveChangesAsync(ct);

        var updated = await classRepository.GetByIdWithDetailsAsync(id, ct);
        return Ok(ToResponseDto(updated!));
    }

    [HttpDelete("{id:long}")]
    public async Task<IActionResult> Delete(long id, CancellationToken ct)
    {
        var schoolClass = await classRepository.GetByIdAsync(id, ct);
        if (schoolClass is null)
        {
            return NotFound();
        }

        classRepository.Remove(schoolClass);
        await classRepository.SaveChangesAsync(ct);

        return NoContent();
    }

    private static SchoolClassResponseDto ToResponseDto(SchoolClass c) => new(
        c.Id, c.ClassCode, c.SubjectId, c.Subject.SubjectName, c.TeacherId, c.Teacher.FullName,
        c.AcademicYear, c.Semester, c.Room, c.Capacity, c.CreatedAt, c.UpdatedAt);
}
