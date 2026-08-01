using EduManagement.Core.Application.DTOs;
using EduManagement.Core.Application.Exceptions;
using EduManagement.Core.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace EduManagement.Api.Controllers;

[ApiController]
[Route("api")]
public class GradesController(GradeService gradeService) : ControllerBase
{
    [HttpGet("grades/{id:long}")]
    public async Task<ActionResult<GradeResponseDto>> GetById(long id, CancellationToken ct)
    {
        var grade = await gradeService.GetByIdAsync(id, ct);
        return grade is null ? NotFound() : Ok(grade);
    }

    [HttpPost("grades")]
    public async Task<ActionResult<GradeResponseDto>> Create(CreateGradeDto dto, CancellationToken ct)
    {
        try
        {
            var created = await gradeService.CreateGradeAsync(dto, ct);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }
        catch (NotFoundException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (ConflictException ex)
        {
            return Conflict(ex.Message);
        }
    }

    [HttpPut("grades/{id:long}")]
    public async Task<ActionResult<GradeResponseDto>> Update(long id, UpdateGradeDto dto, CancellationToken ct)
    {
        try
        {
            return Ok(await gradeService.UpdateGradeAsync(id, dto, ct));
        }
        catch (NotFoundException ex)
        {
            return NotFound(ex.Message);
        }
    }

    [HttpGet("students/{studentId:long}/grades")]
    public async Task<ActionResult<StudentReportDto>> GetStudentReport(long studentId, CancellationToken ct)
    {
        try
        {
            return Ok(await gradeService.GetStudentReportAsync(studentId, ct));
        }
        catch (NotFoundException ex)
        {
            return NotFound(ex.Message);
        }
    }
}
