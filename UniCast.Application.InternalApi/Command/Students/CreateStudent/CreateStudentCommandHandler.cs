using CSharpFunctionalExtensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using UniCast.Application.Abstractions.Persistence;
using UniCast.Application.Result;
using UniCast.Domain.Common.ValueObjects;
using UniCast.Domain.Moodle;
using UniCast.Domain.Students.Entities;
using UniCast.Domain.Students.ValueObjects;

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
            var group = await GetGroupByNameAsync(command.GroupName, ct);
            if (group is null)
            {
                _logger.LogError("Group with name: {GroupName} was not found", command.GroupName);
                return UnitResult.Failure(Error.Of("Group not found", ErrorGroup.NotFound));
            }

            var student = Student.Create(
                id: IdOf<Student>.New(),
                fullName: command.FullName,
                group: group);
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

    private Task<AcademicGroup?> GetGroupByNameAsync(AcademicGroupName name, CancellationToken ct = default)
        => _dataContext.AcademicGroups.SingleOrDefaultAsync(x => x.Name == name, ct);
}