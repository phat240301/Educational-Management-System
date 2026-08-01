using EduManagement.Core.Application.DTOs;
using EduManagement.Core.Application.Interfaces;
using EduManagement.Core.Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace EduManagement.Api.Controllers;

[ApiController]
[Route("api/subjects")]
public class SubjectsController(ISubjectRepository repository) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<SubjectResponseDto>>> GetAll(CancellationToken ct)
    {
        var subjects = await repository.GetAllAsync(ct);
        return Ok(subjects.Select(ToResponseDto));
    }

    [HttpGet("{id:long}")]
    public async Task<ActionResult<SubjectResponseDto>> GetById(long id, CancellationToken ct)
    {
        var subject = await repository.GetByIdAsync(id, ct);
        return subject is null ? NotFound() : Ok(ToResponseDto(subject));
    }

    [HttpPost]
    public async Task<ActionResult<SubjectResponseDto>> Create(CreateSubjectDto dto, CancellationToken ct)
    {
        if (await repository.SubjectCodeExistsAsync(dto.SubjectCode, ct))
        {
            return Conflict($"Subject code '{dto.SubjectCode}' already exists.");
        }

        var subject = new Subject
        {
            SubjectCode = dto.SubjectCode,
            SubjectName = dto.SubjectName,
            CreditHours = dto.CreditHours
        };

        await repository.AddAsync(subject, ct);
        await repository.SaveChangesAsync(ct);

        return CreatedAtAction(nameof(GetById), new { id = subject.Id }, ToResponseDto(subject));
    }

    [HttpPut("{id:long}")]
    public async Task<ActionResult<SubjectResponseDto>> Update(long id, UpdateSubjectDto dto, CancellationToken ct)
    {
        var subject = await repository.GetByIdAsync(id, ct);
        if (subject is null)
        {
            return NotFound();
        }

        subject.SubjectName = dto.SubjectName;
        subject.CreditHours = dto.CreditHours;

        repository.Update(subject);
        await repository.SaveChangesAsync(ct);

        return Ok(ToResponseDto(subject));
    }

    [HttpDelete("{id:long}")]
    public async Task<IActionResult> Delete(long id, CancellationToken ct)
    {
        var subject = await repository.GetByIdAsync(id, ct);
        if (subject is null)
        {
            return NotFound();
        }

        repository.Remove(subject);
        await repository.SaveChangesAsync(ct);

        return NoContent();
    }

    private static SubjectResponseDto ToResponseDto(Subject s) => new(
        s.Id, s.SubjectCode, s.SubjectName, s.CreditHours, s.CreatedAt, s.UpdatedAt);
}
