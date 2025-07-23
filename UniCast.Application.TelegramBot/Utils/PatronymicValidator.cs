namespace UniCast.Application.TelegramBot.Utils;

public static class PatronymicValidator
{
    private static readonly char[] AllowedCharactersWithoutLetters = ['-', ' ', '.', 'I', 'V', '\'', ',', '(', ')'];

    private static bool CharIsRussianLetter(char c)
        => c is >= 'а' and <= 'я' or >= 'А' and <= 'Я';

    public static bool Validate(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
        {
            return false;
        }

        // Проверяем единственный символ
        if (input.Length == 1)
        {
            return CharIsRussianLetter(input[0]);
        }

        // Проверяем первый символ
        if (!CharIsRussianLetter(input[0]) && input[0] != '(')
        {
            return false;
        }

        // Проверяем все символы между первым и последним
        for (int i = 0; i < input.Length - 1; i++)
        {
            if (!CharIsRussianLetter(input[i]) && !AllowedCharactersWithoutLetters.Contains(input[i]))
            {
                return false;
            }

            if (!AllowedCharactersWithoutLetters.Contains(input[i]))
            {
                continue;
            }

            if (AllowedCharactersWithoutLetters.Contains(input[i + 1]))
            {
                return false;
            }
        }

        // Проверяем парность скобок
        if (input.Count(c => c == '(') != input.Count(c => c == ')'))
        {
            return false;
        }

        // Проверяем последний символ
        return CharIsRussianLetter(input[^1]) || input[^1] is '.' or 'I' or 'V';
    }
}