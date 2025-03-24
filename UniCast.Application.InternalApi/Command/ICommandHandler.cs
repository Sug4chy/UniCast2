using CSharpFunctionalExtensions;
using UniCast.Application.Result;

namespace UniCast.Application.InternalApi.Command;

public interface ICommandHandler<in TCommand, TResult>
    where TCommand : ICommand<TResult>
{
    Task<Result<TResult, Error>> HandleAsync(TCommand command, CancellationToken ct = default);
}

public interface ICommandHandler<in TCommand>
    where TCommand : ICommand
{
    Task<UnitResult<Error>> HandleAsync(TCommand command, CancellationToken ct = default);
}