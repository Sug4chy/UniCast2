using FluentValidation;

namespace UniCast.Application.TelegramBot.Utils;

public sealed class PatronymicValidator : AbstractValidator<string>
{
    private static readonly HashSet<char> AllowedCharactersWithoutLetters =
        ['-', Space, Dot, I, V, '\'', ',', OpenBracket, CloseBracket];

    private static readonly HashSet<char> AllowedCharactersWithoutLettersExceptIAndV
        = AllowedCharactersWithoutLetters.Except([I, V]).ToHashSet();

    private static readonly HashSet<char> AllowedCharactersWithoutLettersExceptIAndVAndSpace
        = AllowedCharactersWithoutLetters.Except([I, V, Space]).ToHashSet();

    private const char OpenBracket = '(';
    private const char CloseBracket = ')';
    private const char I = 'I';
    private const char V = 'V';
    private const char Space = ' ';
    private const char Dot = '.';

    public PatronymicValidator()
    {
        RuleFor(x => x).NotEmpty().WithMessage("Отчество не должно быть пустым.");

        // RuleSet("Отчество из единственного символа", () =>
        // {
            RuleFor(x => x)
                .Must(x => CharIsRussianLetter(x[0]))
                .When(x => x.Length == 1)
                .WithMessage("Единственный символ отчества должен быть буквой русского алфавита.");
        // });

        // RuleSet("Отчество из более чем одного символа", () =>
        // {
            RuleFor(x => x[0])
                .Must(x => CharIsRussianLetter(x) || x == OpenBracket)
                .When(x => x.Length > 1)
                .WithMessage("Отчество должно начинаться с буквы русского алфавита или открывающей скобки.");

            RuleForEach(x => x.Index())
                .Must((s, tuple) =>
                {
                    if (tuple.Index == 0 || tuple.Index == s.Length - 1)
                    {
                        return true;
                    }

                    return CharIsRussianLetter(tuple.Item) || AllowedCharactersWithoutLetters.Contains(tuple.Item);
                })
                .When(x => x.Length > 1)
                .OverridePropertyName("Index")
                .WithMessage((_, tuple) =>
                    $"Встречен недопустимый символ на позиции {tuple.Index + 1} ('{tuple.Item}').");

            RuleFor(x => x[x.Length - 1])
                .Must(x => CharIsRussianLetter(x) || x == Dot || x == I || x == V || x == CloseBracket)
                .When(x => x.Length > 1)
                .WithMessage("Последний символ отчества должен быть буквой русского алфавита, точкой, " +
                             "закрывающей скобкой, I или V.");

            RuleForEach(x => x.Index())
                .Must((s, tuple) =>
                {
                    if (tuple.Index == s.Length - 1)
                    {
                        return true;
                    }

                    return !AllowedCharactersWithoutLettersExceptIAndV.Contains(tuple.Item) ||
                           s[tuple.Index + 1] != tuple.Item;
                })
                .When(x => x.Length > 1 && x.Any(c => AllowedCharactersWithoutLetters.Contains(c)))
                .OverridePropertyName("Index")
                .WithMessage("Два символа \".\" (точка), \"-\" (дефис), \"'\" (апостроф), \" \" (пробел), " +
                             "\",\" (запятая) , \"(\" (открывающая скобка), \")\" (закрывающая скобка) не должны идти" +
                             "подряд.");

            RuleForEach(x => x.Index())
                .Must((s, tuple) =>
                {
                    if (tuple.Index == s.Length - 1)
                    {
                        return true;
                    }

                    return !AllowedCharactersWithoutLettersExceptIAndVAndSpace.Contains(tuple.Item) ||
                           !AllowedCharactersWithoutLettersExceptIAndVAndSpace.Contains(s[tuple.Index + 1]);
                })
                .When(x => x.Length > 1 && x.Any(c => AllowedCharactersWithoutLetters.Contains(c)))
                .OverridePropertyName("Index")
                .WithMessage("Символы \".\" (точка), \"-\" (дефис), \"'\" (апостроф), \",\" (запятая), " +
                             "\"(\" (открывающая скобка), \")\" (закрывающая скобка) не должны идти подряд.");

            RuleFor(x => x)
                .Must(x => x.Count(c => c == OpenBracket) == x.Count(c => c == CloseBracket) &&
                           x.IndexOf(OpenBracket) < x.IndexOf(CloseBracket))
                .When(x => x.Length > 1 && (x.Contains(OpenBracket) || x.Contains(CloseBracket)))
                .WithMessage(
                    "Если отчество содержит скобки, то это должна быть правильная скобочная последовательность.");
        // });
    }

    private static bool CharIsRussianLetter(char c)
        => c is >= 'а' and <= 'я' or >= 'А' and <= 'Я';
}