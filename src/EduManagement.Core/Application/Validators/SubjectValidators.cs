using EduManagement.Core.Application.DTOs;
using FluentValidation;

namespace EduManagement.Core.Application.Validators;

public class CreateSubjectDtoValidator : AbstractValidator<CreateSubjectDto>
{
    public CreateSubjectDtoValidator()
    {
        RuleFor(x => x.SubjectCode).NotEmpty().MaximumLength(20);
        RuleFor(x => x.SubjectName).NotEmpty().MaximumLength(150);
        RuleFor(x => x.CreditHours).GreaterThan((short)0);
    }
}

public class UpdateSubjectDtoValidator : AbstractValidator<UpdateSubjectDto>
{
    public UpdateSubjectDtoValidator()
    {
        RuleFor(x => x.SubjectName).NotEmpty().MaximumLength(150);
        RuleFor(x => x.CreditHours).GreaterThan((short)0);
    }
}
