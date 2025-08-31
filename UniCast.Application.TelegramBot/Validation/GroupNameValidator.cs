using System.Linq.Expressions;
using FluentValidation;

namespace UniCast.Application.TelegramBot.Validation;

public sealed class GroupNameValidator : AbstractValidator<string>
{
    private static readonly HashSet<string> NormalizedStudyDirectionsPrefixes = ["при", "пи", "би"];

    private static readonly Expression<Func<string, string>> NormalizedStudyDirectionsPrefix =
        x => x.Split('-', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)[0];

    private static readonly Expression<Func<string, int>> CourseNumber =
        x => int.Parse(x.Split('-', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)[1]) / 100;

    public GroupNameValidator()
    {
        RuleFor(x => x).NotEmpty().WithMessage("Имя группы не должно быть пустым.");

        RuleFor(NormalizedStudyDirectionsPrefix)
            .Must(x => NormalizedStudyDirectionsPrefixes.Contains(x))
            .WithMessage("Неизвестное направление обучения.");

        RuleFor(CourseNumber)
            .Must(x => x is >= 1 and <= 4)
            .WithMessage("Некорректный номер курса.");
    }
}