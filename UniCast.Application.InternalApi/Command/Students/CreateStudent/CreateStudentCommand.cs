using UniCast.Domain.Students.ValueObjects;

namespace UniCast.Application.InternalApi.Command.Students.CreateStudent;

public readonly record struct CreateStudentCommand(
    long Id,
    StudentFullName FullName,
    AcademicGroupName GroupName,
    string Username
) : ICommand;