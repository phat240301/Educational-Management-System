using EduManagement.Core.Application.DTOs;
using EduManagement.Core.Application.Interfaces;
using EduManagement.Core.Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace EduManagement.Api.Controllers;

[ApiController]
[Route("api/teachers")]
public class TeachersController(ITeacherRepository repository) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<TeacherResponseDto>>> GetAll(CancellationToken ct)
    {
        var teachers = await repository.GetAllAsync(ct);
        return Ok(teachers.Select(ToResponseDto));
    }

    [HttpGet("{id:long}")]
    public async Task<ActionResult<TeacherResponseDto>> GetById(long id, CancellationToken ct)
    {
        var teacher = await repository.GetByIdAsync(id, ct);
        return teacher is null ? NotFound() : Ok(ToResponseDto(teacher));
    }

    [HttpPost]
    public async Task<ActionResult<TeacherResponseDto>> Create(CreateTeacherDto dto, CancellationToken ct)
    {
        if (await repository.TeacherCodeExistsAsync(dto.TeacherCode, ct))
        {
            return Conflict($"Teacher code '{dto.TeacherCode}' already exists.");
        }

        if (await repository.EmailExistsAsync(dto.Email, ct))
        {
            return Conflict($"Email '{dto.Email}' already exists.");
        }

        var teacher = new Teacher
        {
            TeacherCode = dto.TeacherCode,
            FullName = dto.FullName,
            Email = dto.Email
        };

        await repository.AddAsync(teacher, ct);
        await repository.SaveChangesAsync(ct);

        return CreatedAtAction(nameof(GetById), new { id = teacher.Id }, ToResponseDto(teacher));
    }

    [HttpPut("{id:long}")]
    public async Task<ActionResult<TeacherResponseDto>> Update(long id, UpdateTeacherDto dto, CancellationToken ct)
    {
        var teacher = await repository.GetByIdAsync(id, ct);
        if (teacher is null)
        {
            return NotFound();
        }

        teacher.FullName = dto.FullName;
        teacher.Email = dto.Email;
        teacher.IsActive = dto.IsActive;

        repository.Update(teacher);
        await repository.SaveChangesAsync(ct);

        return Ok(ToResponseDto(teacher));
    }

    [HttpDelete("{id:long}")]
    public async Task<IActionResult> Delete(long id, CancellationToken ct)
    {
        var teacher = await repository.GetByIdAsync(id, ct);
        if (teacher is null)
        {
            return NotFound();
        }

        repository.Remove(teacher);
        await repository.SaveChangesAsync(ct);

        return NoContent();
    }

    private static TeacherResponseDto ToResponseDto(Teacher t) => new(
        t.Id, t.TeacherCode, t.FullName, t.Email, t.IsActive, t.CreatedAt, t.UpdatedAt);
}
