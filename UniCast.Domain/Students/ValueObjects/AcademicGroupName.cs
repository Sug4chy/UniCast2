using CSharpFunctionalExtensions;

namespace UniCast.Domain.Students.ValueObjects;

public readonly record struct AcademicGroupName
{
    public string StudyDirectionName { get; }
    public int CourseNumber { get; }
    public int GroupNumber { get; }

    private AcademicGroupName(string studyDirectionName, int courseNumber, int groupNumber)
    {
        StudyDirectionName = studyDirectionName;
        CourseNumber = courseNumber;
        GroupNumber = groupNumber;
    }

    public static Result<AcademicGroupName> From(string groupName)
    {
        if (string.IsNullOrWhiteSpace(groupName))
        {
            return Result.Failure<AcademicGroupName>("Имя не должно быть пустым");
        }

        string[] groupNameParts = groupName.Split('-',
            StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        if (groupNameParts.Length != 2 || !int.TryParse(groupNameParts[1], out _))
        {
            return Result.Failure<AcademicGroupName>("Не валидный формат ввода");
        }

        return Result.Success(new AcademicGroupName(
            studyDirectionName: groupNameParts[0],
            courseNumber: groupNameParts[1][0] - '0',
            groupNumber: int.Parse(groupNameParts[1][1..])));
    }
}