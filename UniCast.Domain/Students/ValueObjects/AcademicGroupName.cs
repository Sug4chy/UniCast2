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

    public static bool IsValid(string groupName)
    {
        string[] groupNameParts = groupName.Split('-', 
            StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

        return !string.IsNullOrWhiteSpace(groupName) &&
               groupNameParts.Length == 2 &&
               int.TryParse(groupNameParts[1], out _);
    }


    public static AcademicGroupName From(string groupName)
    {
        if (!IsValid(groupName))
        {
            throw new ArgumentException(groupName, nameof(groupName));
        }

        string[] groupNameParts = groupName.Split('-', 
            StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

        return new AcademicGroupName(
            studyDirectionName: groupNameParts[0],
            courseNumber: groupNameParts[1][0] - '0',
            groupNumber: int.Parse(groupNameParts[1][1..]));
    }

    public static implicit operator string(AcademicGroupName groupName) => groupName.ToString();

    public override string ToString()
        => $"{StudyDirectionName}-{CourseNumber * 100 + GroupNumber}";
}