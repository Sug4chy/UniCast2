using CSharpFunctionalExtensions;

namespace UniCast.Domain.Students.ValueObjects;

public readonly record struct AcademicGroupName
{
    private readonly string _value;

    private AcademicGroupName(string value)
    {
        _value = value;
    }

    public static Result<AcademicGroupName> Create(string value)
    {
        string[] valueParts = value.Split('-',
            StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        int groupNumber = int.Parse(valueParts[1]);

        return groupNumber % 100 is < 1 or > 4
            ? Result.Failure<AcademicGroupName>("Номер группы некорректен")
            : Result.Success(new AcademicGroupName(value));
    }

    public static implicit operator string(AcademicGroupName groupName)
        => groupName._value;

    public override string ToString()
        => this;
}