using FluentValidation;
using UniCast.Domain.Students.ValueObjects;

namespace UniCast.InternalApi.Dto;

public readonly record struct StudentDto(
    string FullName,
    string GroupName
);

public sealed class StudentDtoValidator : AbstractValidator<StudentDto>
{
    public StudentDtoValidator()
    {
        RuleFor(x => x.FullName)
            .NotEmpty()
            .Must(StudentFullName.IsValid);

        RuleFor(x => x.GroupName)
            .NotEmpty()
            .Must(AcademicGroupName.IsValid);
    }
}