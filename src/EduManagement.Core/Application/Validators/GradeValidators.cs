using EduManagement.Core.Application.DTOs;
using FluentValidation;

namespace EduManagement.Core.Application.Validators;

public class CreateGradeDtoValidator : AbstractValidator<CreateGradeDto>
{
    public CreateGradeDtoValidator()
    {
        RuleFor(x => x.StudentId).GreaterThan(0L);
        RuleFor(x => x.ClassId).GreaterThan(0L);
        RuleFor(x => x.AttendanceScore).InclusiveBetween(0m, 100m).When(x => x.AttendanceScore.HasValue);
        RuleFor(x => x.MidtermScore).InclusiveBetween(0m, 100m).When(x => x.MidtermScore.HasValue);
        RuleFor(x => x.FinalScore).InclusiveBetween(0m, 100m).When(x => x.FinalScore.HasValue);
    }
}

public class UpdateGradeDtoValidator : AbstractValidator<UpdateGradeDto>
{
    public UpdateGradeDtoValidator()
    {
        RuleFor(x => x.AttendanceScore).InclusiveBetween(0m, 100m).When(x => x.AttendanceScore.HasValue);
        RuleFor(x => x.MidtermScore).InclusiveBetween(0m, 100m).When(x => x.MidtermScore.HasValue);
        RuleFor(x => x.FinalScore).InclusiveBetween(0m, 100m).When(x => x.FinalScore.HasValue);
    }
}
