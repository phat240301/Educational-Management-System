using EduManagement.Core.Application.DTOs;
using FluentValidation;

namespace EduManagement.Core.Application.Validators;

public class CreateEnrollmentDtoValidator : AbstractValidator<CreateEnrollmentDto>
{
    public CreateEnrollmentDtoValidator()
    {
        RuleFor(x => x.StudentId).GreaterThan(0L);
        RuleFor(x => x.ClassId).GreaterThan(0L);
    }
}

public class UpdateEnrollmentStatusDtoValidator : AbstractValidator<UpdateEnrollmentStatusDto>
{
    public UpdateEnrollmentStatusDtoValidator()
    {
        RuleFor(x => x.Status).IsInEnum();
    }
}
