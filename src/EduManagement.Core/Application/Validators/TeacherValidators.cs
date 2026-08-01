using EduManagement.Core.Application.DTOs;
using FluentValidation;

namespace EduManagement.Core.Application.Validators;

public class CreateTeacherDtoValidator : AbstractValidator<CreateTeacherDto>
{
    public CreateTeacherDtoValidator()
    {
        RuleFor(x => x.TeacherCode).NotEmpty().MaximumLength(20);
        RuleFor(x => x.FullName).NotEmpty().MaximumLength(150);
        RuleFor(x => x.Email).NotEmpty().EmailAddress().MaximumLength(200);
    }
}

public class UpdateTeacherDtoValidator : AbstractValidator<UpdateTeacherDto>
{
    public UpdateTeacherDtoValidator()
    {
        RuleFor(x => x.FullName).NotEmpty().MaximumLength(150);
        RuleFor(x => x.Email).NotEmpty().EmailAddress().MaximumLength(200);
    }
}
