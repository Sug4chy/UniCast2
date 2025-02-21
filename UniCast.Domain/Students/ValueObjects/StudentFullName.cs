using System.Collections.Immutable;
using System.Text;
using CSharpFunctionalExtensions;

namespace UniCast.Domain.Students.ValueObjects;

public readonly record struct StudentFullName
{
    private static readonly ImmutableArray<string> FullNamePartsNames = ["Фамилия", "Имя"];

    public string Name { get; }
    public string Surname { get; }
    public Maybe<string> Patronymic { get; }

    private StudentFullName(string name, string surname, Maybe<string> patronymic)
    {
        Name = name;
        Surname = surname;
        Patronymic = patronymic;
    }

    public static Result<StudentFullName> Create(string fullName)
    {
        string[] fullNameParts = fullName.Split(' ',
            StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        if (fullNameParts.Length != 3)
        {
            return Result.Failure<StudentFullName>("ФИО должно состоять из трёх частей");
        }

        for (int i = 0; i < 2; i++)
        {
            if (string.IsNullOrEmpty(fullNameParts[i]))
            {
                return Result.Failure<StudentFullName>($"{FullNamePartsNames[i]} отсутствует");
            }
        }

        return Result.Success(new StudentFullName(
            name: fullNameParts[0],
            surname: fullNameParts[1],
            patronymic: Maybe.From(fullNameParts[2])));
    }

    public static implicit operator string(StudentFullName studentFullName)
    {
        var sb = new StringBuilder();
        sb.Append($"{studentFullName.Surname} {studentFullName.Name}");
        if (studentFullName.Patronymic.HasValue)
        {
            sb.Append($" {studentFullName.Patronymic}");
        }

        return sb.ToString();
    }

    public override string ToString() => this;
}