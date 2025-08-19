using FluentValidation;

namespace UniCast.Application.TelegramBot.Validation;

public sealed class GroupNameValidator : AbstractValidator<string>
{
    private static readonly HashSet<string> NormalizedStudyDirectionsPrefixes = ["при", "пи", "би"];

    public GroupNameValidator()
    {
        RuleFor(x => x).NotEmpty().WithMessage("Имя группы не должно быть пустым.");

        RuleFor(x => x);
    }
}