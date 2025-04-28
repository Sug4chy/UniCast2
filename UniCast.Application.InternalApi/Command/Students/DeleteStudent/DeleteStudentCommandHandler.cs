using CSharpFunctionalExtensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using UniCast.Application.Abstractions.Persistence;
using UniCast.Application.Result;

namespace UniCast.Application.InternalApi.Command.Students.DeleteStudent;

public sealed class DeleteStudentCommandHandler : ICommandHandler<DeleteStudentCommand>
{
    private readonly IDataContext _dataContext;
    private readonly ILogger<DeleteStudentCommandHandler> _logger;

    public DeleteStudentCommandHandler(IDataContext dataContext, ILogger<DeleteStudentCommandHandler> logger)
    {
        _dataContext = dataContext;
        _logger = logger;
    }

    public async Task<UnitResult<Error>> HandleAsync(DeleteStudentCommand command, CancellationToken ct = default)
    {
        try
        {
            var account = await _dataContext.MoodleAccounts
                .Include(x => x.Student)
                .SingleOrDefaultAsync(x => x.ExtId == command.Id, ct);
            if (account is null)
            {
                _logger.LogError("Account with id {ExtId} was not found", command.Id);
                return UnitResult.Failure(Error.Of("Account not found", ErrorGroup.NotFound));
            }

            _dataContext.MoodleAccounts.Remove(account);
            _dataContext.Students.Remove(account.Student!);

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