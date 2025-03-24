using System.Text;

namespace UniCast.Domain.Students.ValueObjects;

public readonly record struct StudentFullName
{
    public string Name { get; }
    public string Surname { get; }
    public string? Patronymic { get; }

    private StudentFullName(string name, string surname, string? patronymic = null)
    {
        Name = name;
        Surname = surname;
        Patronymic = patronymic;
    }

    public static bool IsValid(string fullName)
        => fullName.Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries).Length
            is > 1 and <= 3;

    public static StudentFullName From(string fullName)
    {
        if (!IsValid(fullName))
        {
            throw new ArgumentException(fullName, nameof(fullName));
        }

        string[] fullNameParts = fullName.Split(' ',
            StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

        return new StudentFullName(
            name: fullNameParts[1],
            surname: fullNameParts[0],
            patronymic: fullNameParts.Length > 2 ? fullNameParts[2] : null
        );
    }

    public static implicit operator string(StudentFullName studentFullName)
        => studentFullName.ToString();

    public override string ToString()
    {
        var sb = new StringBuilder();
        sb.Append($"{Surname} {Name}");
        if (Patronymic is not null)
        {
            sb.Append($" {Patronymic}");
        }

        return sb.ToString();
    }
}