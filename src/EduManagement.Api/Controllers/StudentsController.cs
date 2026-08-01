using EduManagement.Core.Application.DTOs;
using EduManagement.Core.Application.Interfaces;
using EduManagement.Core.Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace EduManagement.Api.Controllers;

[ApiController]
[Route("api/students")]
public class StudentsController(IStudentRepository repository) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<StudentResponseDto>>> GetAll(CancellationToken ct)
    {
        var students = await repository.GetAllAsync(ct);
        return Ok(students.Select(ToResponseDto));
    }

    [HttpGet("{id:long}")]
    public async Task<ActionResult<StudentResponseDto>> GetById(long id, CancellationToken ct)
    {
        var student = await repository.GetByIdAsync(id, ct);
        return student is null ? NotFound() : Ok(ToResponseDto(student));
    }

    [HttpPost]
    public async Task<ActionResult<StudentResponseDto>> Create(CreateStudentDto dto, CancellationToken ct)
    {
        if (await repository.StudentCodeExistsAsync(dto.StudentCode, ct))
        {
            return Conflict($"Student code '{dto.StudentCode}' already exists.");
        }

        var student = new Student
        {
            StudentCode = dto.StudentCode,
            FullName = dto.FullName,
            DateOfBirth = dto.DateOfBirth,
            ParentEmail = dto.ParentEmail,
            ParentPhone = dto.ParentPhone
        };

        await repository.AddAsync(student, ct);
        await repository.SaveChangesAsync(ct);

        return CreatedAtAction(nameof(GetById), new { id = student.Id }, ToResponseDto(student));
    }

    [HttpPut("{id:long}")]
    public async Task<ActionResult<StudentResponseDto>> Update(long id, UpdateStudentDto dto, CancellationToken ct)
    {
        var student = await repository.GetByIdAsync(id, ct);
        if (student is null)
        {
            return NotFound();
        }

        student.FullName = dto.FullName;
        student.DateOfBirth = dto.DateOfBirth;
        student.ParentEmail = dto.ParentEmail;
        student.ParentPhone = dto.ParentPhone;
        student.IsActive = dto.IsActive;

        repository.Update(student);
        await repository.SaveChangesAsync(ct);

        return Ok(ToResponseDto(student));
    }

    [HttpDelete("{id:long}")]
    public async Task<IActionResult> Delete(long id, CancellationToken ct)
    {
        var student = await repository.GetByIdAsync(id, ct);
        if (student is null)
        {
            return NotFound();
        }

        repository.Remove(student);
        await repository.SaveChangesAsync(ct);

        return NoContent();
    }

    private static StudentResponseDto ToResponseDto(Student s) => new(
        s.Id, s.StudentCode, s.FullName, s.DateOfBirth, s.ParentEmail, s.ParentPhone,
        s.IsActive, s.CreatedAt, s.UpdatedAt);
}
