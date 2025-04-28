namespace UniCast.Application.InternalApi.Command.Students.DeleteStudent;

public readonly record struct DeleteStudentCommand(long Id) : ICommand;