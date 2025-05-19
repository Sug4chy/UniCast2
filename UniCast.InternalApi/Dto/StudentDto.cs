using FluentValidation;

namespace UniCast.InternalApi.Dto;

public readonly record struct StudentDto(
    long Id
);

public sealed class StudentDtoValidator : AbstractValidator<StudentDto>
{
    public StudentDtoValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .GreaterThan(0);
    }
}