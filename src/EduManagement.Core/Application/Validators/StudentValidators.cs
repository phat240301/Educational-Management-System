using EduManagement.Core.Application.DTOs;
using FluentValidation;

namespace EduManagement.Core.Application.Validators;

public class CreateStudentDtoValidator : AbstractValidator<CreateStudentDto>
{
    public CreateStudentDtoValidator()
    {
        RuleFor(x => x.StudentCode).NotEmpty().MaximumLength(20);
        RuleFor(x => x.FullName).NotEmpty().MaximumLength(150);
        RuleFor(x => x.DateOfBirth).LessThan(DateOnly.FromDateTime(DateTime.UtcNow));
        RuleFor(x => x.ParentEmail).NotEmpty().EmailAddress().MaximumLength(200);
        RuleFor(x => x.ParentPhone).MaximumLength(20);
    }
}

public class UpdateStudentDtoValidator : AbstractValidator<UpdateStudentDto>
{
    public UpdateStudentDtoValidator()
    {
        RuleFor(x => x.FullName).NotEmpty().MaximumLength(150);
        RuleFor(x => x.DateOfBirth).LessThan(DateOnly.FromDateTime(DateTime.UtcNow));
        RuleFor(x => x.ParentEmail).NotEmpty().EmailAddress().MaximumLength(200);
        RuleFor(x => x.ParentPhone).MaximumLength(20);
    }
}
