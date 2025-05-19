using CSharpFunctionalExtensions;
using Microsoft.Extensions.Logging;
using UniCast.Application.Abstractions.Persistence;
using UniCast.Application.Result;
using UniCast.Domain.Common.ValueObjects;
using UniCast.Domain.Moodle;
using UniCast.Domain.Students.Entities;

namespace UniCast.Application.InternalApi.Command.Students.CreateStudent;

public sealed class CreateStudentCommandHandler : ICommandHandler<CreateStudentCommand>
{
    private readonly IDataContext _dataContext;
    private readonly ILogger<CreateStudentCommandHandler> _logger;

    public CreateStudentCommandHandler(IDataContext dataContext, ILogger<CreateStudentCommandHandler> logger)
    {
        _dataContext = dataContext;
        _logger = logger;
    }

    public async Task<UnitResult<Error>> HandleAsync(CreateStudentCommand command, CancellationToken ct = default)
    {
        try
        {
            var student = Student.Create(
                id: IdOf<Student>.New(),
                fullName: command.FullName);
            var moodleAccount = new MoodleAccount(IdOf<MoodleAccount>.New())
            {
                ExtId = command.Id,
                Username = command.Username,
                StudentId = student.Id,
                Student = student,
            };

            _dataContext.Students.Add(student);
            _dataContext.MoodleAccounts.Add(moodleAccount);

            await _dataContext.SaveChangesAsync(ct);

            return UnitResult.Success<Error>();
        }
        catch (Exception e)
        {
            _logger.LogError(e, e.Message);
            return UnitResult.Failure(Error.Of(e.Message));
        }
    }
}