using EduManagement.Core.Application.DTOs;
using FluentValidation;

namespace EduManagement.Core.Application.Validators;

public class CreateSchoolClassDtoValidator : AbstractValidator<CreateSchoolClassDto>
{
    public CreateSchoolClassDtoValidator()
    {
        RuleFor(x => x.ClassCode).NotEmpty().MaximumLength(30);
        RuleFor(x => x.SubjectId).GreaterThan(0L);
        RuleFor(x => x.TeacherId).GreaterThan(0L);
        RuleFor(x => x.AcademicYear).NotEmpty().Matches(@"^\d{4}-\d{4}$")
            .WithMessage("AcademicYear must be in the form 'YYYY-YYYY'.");
        RuleFor(x => x.Semester).InclusiveBetween((short)1, (short)2);
        RuleFor(x => x.Room).MaximumLength(30);
        RuleFor(x => x.Capacity).GreaterThan((short)0);
    }
}

public class UpdateSchoolClassDtoValidator : AbstractValidator<UpdateSchoolClassDto>
{
    public UpdateSchoolClassDtoValidator()
    {
        RuleFor(x => x.SubjectId).GreaterThan(0L);
        RuleFor(x => x.TeacherId).GreaterThan(0L);
        RuleFor(x => x.AcademicYear).NotEmpty().Matches(@"^\d{4}-\d{4}$")
            .WithMessage("AcademicYear must be in the form 'YYYY-YYYY'.");
        RuleFor(x => x.Semester).InclusiveBetween((short)1, (short)2);
        RuleFor(x => x.Room).MaximumLength(30);
        RuleFor(x => x.Capacity).GreaterThan((short)0);
    }
}
